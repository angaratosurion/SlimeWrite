using System.Text;
using SevenZip.Compression.LZMA;
using Encoder = SevenZip.Compression.LZMA.Encoder;

namespace SlimeLzma;

internal static class LzmaArchive
{
    //private const string MAGIC = "SLZMA1";

    // ================= FILE =================
    //public static void WriteFile(Stream input, Stream output)
    //{
    //    using var bw = new BinaryWriter(output, Encoding.UTF8, true);

    //    bw.Write(MAGIC);

    //    var encoder = new Encoder();
    //    LzmaOptions.ApplyMaxCompression(encoder);

    //    encoder.WriteCoderProperties(output);

    //    bw.Write(input.Length);

    //    encoder.Code(input, output, input.Length, -1, null);
    //}

    // ================= DIRECTORY =================
    public static void WriteDirectory(string folder, Stream output)
    {
        using var bw = new BinaryWriter(output, Encoding.UTF8, true);

        // MAGIC (fixed bytes)
        bw.Write(new byte[] { 0x5A, 0x4C, 0x4D, 0x31 }); // ZLM1

        // VERSION (CRITICAL FIX)
        bw.Write((byte)1);

        var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
        bw.Write(files.Length);

        var encoder = new SevenZip.Compression.LZMA.Encoder();
        LzmaOptions.ApplyMaxCompression(encoder);

        foreach (var file in files)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(Path.GetRelativePath(folder, file));
            byte[] data = File.ReadAllBytes(file);

            using var msIn = new MemoryStream(data);
            using var msOut = new MemoryStream();

            encoder.Code(msIn, msOut, data.Length, -1, null);

            byte[] compressed = msOut.ToArray();

            bw.Write(nameBytes.Length);
            bw.Write(nameBytes);

            bw.Write(data.Length);
            bw.Write(compressed.Length);
            bw.Write(compressed);
        }
    }
    // ================= EXTRACT =================
    public static void Extract(Stream input, string outputFolder)
    {
        using var br = new BinaryReader(input, Encoding.UTF8, true);

        // MAGIC CHECK (STRICT)
        byte[] magic = br.ReadBytes(4);

        if (magic.Length != 4 ||
            magic[0] != 0x5A ||
            magic[1] != 0x4C ||
            magic[2] != 0x4D ||
            magic[3] != 0x31)
            throw new InvalidDataException("Invalid archive");

        // VERSION CHECK (CRITICAL)
        byte version = br.ReadByte();
        if (version != 1)
            throw new InvalidDataException("Unsupported version");

        int fileCount = br.ReadInt32();

        if (fileCount < 0 || fileCount > 1_000_000)
            throw new InvalidDataException("Corrupted file count");

        Directory.CreateDirectory(outputFolder);

        var decoder = new SevenZip.Compression.LZMA.Decoder();

        for (int i = 0; i < fileCount; i++)
        {
            int nameLen = br.ReadInt32();

            if (nameLen <= 0 || nameLen > 10000)
                throw new InvalidDataException("Corrupted nameLen");

            string name = Encoding.UTF8.GetString(br.ReadBytes(nameLen));

            int originalSize = br.ReadInt32();
            int compressedSize = br.ReadInt32();

            if (compressedSize <= 0 || compressedSize > 1_000_000_000)
                throw new InvalidDataException("Corrupted compressedSize");

            byte[] compressed = br.ReadBytes(compressedSize);

            if (compressed.Length != compressedSize)
                throw new InvalidDataException("Stream desync");

            using var ms = new MemoryStream(compressed);

            byte[] props = new byte[5];
            ms.Read(props, 0, 5);

            decoder.SetDecoderProperties(props);

            string outPath = Path.Combine(outputFolder, name);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);

            using var fs = File.Create(outPath);

            decoder.Code(ms, fs, ms.Length - ms.Position, originalSize, null);
        }
    }
    static int ReadInt(BinaryReader br)
    {
        byte[] b = br.ReadBytes(4);

        if (b.Length != 4)
            throw new InvalidDataException("Unexpected EOF");

        return BitConverter.ToInt32(b, 0);
    }
}