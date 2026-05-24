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

        foreach (var file in files)
        {
            string relative = Path.GetRelativePath(folder, file);
            byte[] nameBytes = Encoding.UTF8.GetBytes(relative);
            byte[] data = File.ReadAllBytes(file);

            bw.Write(nameBytes.Length);
            bw.Write(nameBytes);

            bw.Write(data.Length);

            var encoder = new Encoder();
            LzmaOptions.ApplyMaxCompression(encoder);

            encoder.WriteCoderProperties(output);

            using var ms = new MemoryStream(data);
            encoder.Code(ms, output, data.Length, -1, null);
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
            string name = Encoding.UTF8.GetString(br.ReadBytes(nameLen));

            int size = br.ReadInt32();
            byte[] props = br.ReadBytes(5);

            var decoder = new SevenZip.Compression.LZMA.Decoder();
            decoder.SetDecoderProperties(props);

            string outPath = Path.Combine(outputFolder, name);
            Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);

            using var outStream = File.Create(outPath);

            decoder.Code(input, outStream, input.Length, size, null);
        }
    }
}