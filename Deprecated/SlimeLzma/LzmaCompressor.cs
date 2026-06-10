using System.Text;

namespace SlimeLzma;

public static class LzmaCompressor
{
    //public static void CompressFile(string inputFile, string outputFile)
    //{
    //    using var input = File.OpenRead(inputFile);
    //    using var output = File.Create(outputFile);

    //    LzmaArchive.WriteFile(input, output);
    //}

    public static void CompressDirectory(string folder, string outputFile)
    {
        using var output = File.Create(outputFile);
        LzmaArchive.WriteDirectory(folder, output);
    }

    public static void Extract(string inputFile, string outputFolder)
    {
        using var input = File.OpenRead(inputFile);
        LzmaArchive.Extract(input, outputFolder);
    }
}