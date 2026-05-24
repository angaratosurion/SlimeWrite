using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Readers;
using SharpCompress.Writers;
using SlimeWrite.Core.Helpers;

namespace SlimeWrite.Archive;

public static class Slime7z
{
    // ================= CREATE =================
    public static async void Create(string folder, string outputFile)
    {
        try
        {
            WriterOptions writerOptions = new 
                WriterOptions(CompressionType.LZMA2,22)
            {
                ArchiveEncoding = new ArchiveEncoding()
                {
                    Default = System.Text.Encoding.UTF8
                },
                

            };
             
            using Stream stream = File.OpenWrite(outputFile);
            await using var writer = await WriterFactory.OpenAsyncWriter(stream, ArchiveType.SevenZip,
                writerOptions);
            await writer.WriteAllAsync(
                @"D:\files",
                "*",
                SearchOption.AllDirectories
            );
        }
        catch (Exception ex)
        {
            StaticVariables.core.ErrorLog(ex);

        }
    }

    // ================= EXTRACT =================
    public static async void Extract(string file, string outputFolder)
    {

        try
        {
            using Stream stream = File.OpenRead(file);
            await using var reader = await ReaderFactory.OpenAsyncReader(stream);
            await reader.WriteAllToDirectoryAsync(
                outputFolder);//,
                              // cancellationToken: cancellationToken
                              // );

        }
        catch (Exception ex)
        {
           StaticVariables.core.ErrorLog(ex);
             
        }
    }
}