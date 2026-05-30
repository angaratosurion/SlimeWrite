using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Factories;
using SharpCompress.Readers;
using SharpCompress.Writers;
using SlimeWrite.Core.Helpers;

namespace SlimeWrite.Core.Archive;

public static class Slime7z
{
    // ================= CREATE =================
    public static async void Create(string folder, string outputFile)
    {
        try
        {
            WriterOptions writerOptions = new 
                WriterOptions(CompressionType.LZMA2,0)
            {
                ArchiveEncoding = new ArchiveEncoding()
                {
                    Default = System.Text.Encoding.UTF8
                },
                

            };
               
            using Stream stream = File.OpenWrite(outputFile);
            await using var writer = await WriterFactory.
                OpenAsyncWriter(stream, ArchiveType.SevenZip,
                writerOptions);
             var outputFilehtml = Path.Combine(folder, "output.html");
             if ( File.Exists(outputFilehtml) == false)
            {
                File.Delete(outputFilehtml);
            }

            await writer.WriteAllAsync(
                folder,
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
            ReaderOptions readerOptions = new ReaderOptions()
            {
                ArchiveEncoding = new ArchiveEncoding()
                {
                    Default = System.Text.Encoding.UTF8
                },
               
            };
             SevenZipFactory sevenZipFactory =  new SevenZipFactory();
            
            
            //  await using var reader = await ReaderFactory.OpenAsyncReader(stream);
            await using var reader = await sevenZipFactory.
                OpenAsyncArchive(stream,
                readerOptions); 
            if ( Directory.Exists(outputFolder) ==false)
            {
                Directory.CreateDirectory(outputFolder);
            }
             await reader.WriteToDirectoryAsync(outputFolder);

             
            //await reader.WriteAllToDirectoryAsync(
            //    outputFolder);//,
                              // cancellationToken: cancellationToken
                              // );

        }
        catch (Exception ex)
        {
           StaticVariables.core.ErrorLog(ex);
             
        }
    }
}