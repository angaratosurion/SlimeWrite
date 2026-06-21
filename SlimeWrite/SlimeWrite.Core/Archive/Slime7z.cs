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
    public static async Task   Create(string folder, string outputFile)
    {
        try
        {
            WriterOptions writerOptions = new 
                WriterOptions(CompressionType.LZMA,0)
            {
                ArchiveEncoding = new ArchiveEncoding()
                {
                    Default = System.Text.Encoding.UTF8
                }, 
                

            };
             
            //using Stream stream = File.OpenWrite(outputFile);
            await using var writer = await WriterFactory.
                OpenAsyncWriter(outputFile, ArchiveType.SevenZip,
                writerOptions);
            //await using var writer = await WriterFactory.
            //    OpenAsyncWriter(stream,ArchiveType.SevenZip,
            //    writerOptions);
            var outputFilehtml = Path.Combine(folder, "output.html");
             if ( File.Exists(outputFilehtml) != false)
            {
                File.Delete(outputFilehtml);
            }

           
            await writer.WriteAllAsync(
                directory: folder,
                searchPattern: "*",
                fileSearchFunc: file =>
                    {
        // Skip the file if it matches your target output file path
                    return !string.Equals(file, outputFile, 
                        StringComparison.OrdinalIgnoreCase);
                       
                },
    option: SearchOption.AllDirectories
);
          //  stream.Close();
        }
        catch (Exception ex)
        {
            StaticVariables.core.ErrorLog(ex);

        }
    }

    // ================= EXTRACT =================
    public static   void Extract(string file, string outputFolder)
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
            SevenZipFactory sevenZipFactory = new SevenZipFactory();


            // await using var reader = await ReaderFactory.OpenAsyncReader(stream);
            var reader = sevenZipFactory.
             OpenArchive(stream,
             readerOptions);
            if (Directory.Exists(outputFolder) == false)
            {
                Directory.CreateDirectory(outputFolder);
            }
            ExtractionOptions extractionOptions = new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = true
            };


            reader.WriteToDirectory(
               outputFolder, extractionOptions, null);
        

            

            //await reader.WriteAllToDirectoryAsync(
             //   outputFolder);//,
        //cancellationToken: cancellationToken
        //                       );

        }
        catch (Exception ex)
        {
           StaticVariables.core.ErrorLog(ex);
             
        }
    }
}