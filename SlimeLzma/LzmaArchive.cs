using System.Text;
using SevenZip.Compression.LZMA;
using Encoder = SevenZip.Compression.LZMA.Encoder;

namespace SlimeLzma;

internal static class LzmaArchive
{
    private const string MAGIC = "SLZMA1";

    // ================= FILE =================
    public static void WriteFile(Stream input, Stream output)
    {
        using var bw = new BinaryWriter(output, Encoding.UTF8, true);

        bw.Write(MAGIC);

        var encoder = new Encoder();
        LzmaOptions.ApplyMaxCompression(encoder);

        encoder.WriteCoderProperties(output);

        bw.Write(input.Length);

        encoder.Code(input, output, input.Length, -1, null);
    }

    // ================= DIRECTORY =================
    public static void WriteDirectory(string folder, Stream output)
    {
        using var bw = new BinaryWriter(output, Encoding.UTF8, true);

        bw.Write(MAGIC);

        var files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
        bw.Write(files.Length);

        var encoder = new SevenZip.Compression.LZMA.Encoder();
        LzmaOptions.ApplyMaxCompression(encoder);

        foreach (var file in files)
        {
            string relative = Path.GetRelativePath(folder, file);
            byte[] nameBytes = Encoding.UTF8.GetBytes(relative);
            byte[] data = File.ReadAllBytes(file);

            // ---------------- FIX: compress cleanly ----------------
            using var compressedStream = new MemoryStream();

            using (var ms = new MemoryStream(data))
            {
                encoder.Code(ms, compressedStream, data.Length, -1, null);
            }

            byte[] compressed = compressedStream.ToArray();

            // ---------------- VALIDATION ----------------
            if (compressed.Length <= 0)
                throw new Exception("Compression failed");

            // ---------------- WRITE ----------------
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

        var magic = new string(br.ReadChars(6));
        if (!magic.StartsWith("SLZMA"))
            throw new InvalidDataException("Invalid archive");

        int fileCount = br.ReadInt32();
        Directory.CreateDirectory(outputFolder);

        for (int i = 0; i < fileCount; i++)
        {
            int nameLen = br.ReadInt32();
            if (nameLen <= 0 || nameLen > 10000)
                throw new InvalidDataException("Corrupted nameLen");

            string name = Encoding.UTF8.GetString(br.ReadBytes(nameLen));

            int originalSize = br.ReadInt32();
            int compressedSize = br.ReadInt32();

            if (compressedSize <= 0 || compressedSize > 1_000_000_000)
                throw new InvalidDataException($"Corrupted compressedSize: {compressedSize}");

            byte[] compressedData = br.ReadBytes(compressedSize);

            using var inStream = new MemoryStream(compressedData);

            var decoder = new SevenZip.Compression.LZMA.Decoder();

            // IMPORTANT: LZMA expects properties BEFORE data
            byte[] props = new byte[5];
            inStream.Read(props, 0, 5);

            decoder.SetDecoderProperties(props);

            string outPath = Path.Combine(outputFolder, name);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);

            using var outStream = File.Create(outPath);

            decoder.Code(
                inStream,
                outStream,
                compressedData.Length,
                originalSize,
                null);
        }
    }
}