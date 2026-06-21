#if ANDROID
using Android.Content;
using Android.OS;
using Android.Provider;
using SlimeWrite.Core.Models;

#endif
using System.IO;
#if ANDROID
namespace SlimeWrite.Core.Helpers
{
  
    public static class FileCopier
    {
        public static void CopyFolderToDownloads(string sourceFolder, string targetSubFolder, 
            DocumentInfo document)
        {


            var context = Android.App.Application.Context;

            var files = Directory.GetFiles(sourceFolder);

            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file);
                byte[] data = File.ReadAllBytes(file);
                if (Path.GetExtension(file).ToLower() != ".html")
                {
                    ContentValues values = new ContentValues();

                    values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
                    values.Put(MediaStore.IMediaColumns.MimeType, "application/octet-stream");
                    values.Put(MediaStore.IMediaColumns.RelativePath, "Download/" + 
                        StaticVariables.core.GetAppInfo().AppName +
                    "/Docs/"
                    + targetSubFolder);

                    var uri = context.ContentResolver.Insert(
                        MediaStore.Downloads.ExternalContentUri,
                        values);

                    if (uri == null)
                        continue;
                    
                    using var stream = context.ContentResolver.OpenOutputStream(uri);
                    stream!.Write(data, 0, data.Length);
                }
            }


        }
        public static void CopyFileToDownloads(string sourceFilePath, 
            string targetFileName)
        {
            var context = Android.App.Application.Context;

            // 1. Διαβάζουμε τα bytes από το sourceFilePath (π.χ. το temp αρχείο στην cache)
            byte[] data = File.ReadAllBytes(sourceFilePath);

            // 2. Απομονώνουμε μόνο το καθαρό όνομα αρχείου (π.χ. "test.7zsmd")
            string fileName = Path.GetFileName(targetFileName);
            string appName = StaticVariables.core.GetAppInfo().AppName;

            ContentValues values = new ContentValues();
            values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            values.Put(MediaStore.IMediaColumns.MimeType, "application/octet-stream");

            // Ορίζουμε τη διαδρομή μέσα στα Downloads του Android
            values.Put(MediaStore.IMediaColumns.RelativePath, $"Download/{appName}/Docs");

            var uri = context.ContentResolver.Insert(
                MediaStore.Downloads.ExternalContentUri,
                values);

            if (uri != null)
            {
                using (var stream = context.ContentResolver.OpenOutputStream(uri))
                {
                    if (stream != null)
                    {
                        stream.Write(data, 0, data.Length);
                        stream.Flush(); // Εξασφαλίζουμε ότι γράφτηκαν όλα τα δεδομένα
                    }
                }
            }
        }
        public static void CopyFileLogToDownloads(string sourceFolder, string file)
        {


            var context = Android.App.Application.Context;



            string fileName = Path.GetFileName(file);
            byte[] data = File.ReadAllBytes(file);
            string appath = StaticVariables.core.GetAppInfo().AppName;
            string targetSubFolder = Path.Combine( "Logs", fileName);


            ContentValues values = new ContentValues();

            values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
            values.Put(MediaStore.IMediaColumns.MimeType, "application/octet-stream");
            // values.Put(MediaStore.IMediaColumns.RelativePath, "Download/" + file);
            values.Put(MediaStore.IMediaColumns.RelativePath, "Download/" +
                StaticVariables.core.GetAppInfo().AppName+"/"
                + targetSubFolder);


            var uri = context.ContentResolver.Insert(
                MediaStore.Downloads.ExternalContentUri,
                values);

             if (File.Exists(targetSubFolder))
            {
                File.Delete(targetSubFolder);
            }

            using var stream = context.ContentResolver.OpenOutputStream(uri);
            stream!.Write(data, 0, data.Length);
        }


    }



}
#endif
