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
        public static void CopyFileToDownloads(string sourceFolder, string file)
        {


            var context = Android.App.Application.Context;

            
           
                string fileName = Path.GetFileName(file);
                byte[] data = File.ReadAllBytes(file);
            string appath = StaticVariables.core.GetAppInfo().AppName;
            string targetSubFolder = Path.Combine(appath,"Logs",fileName);
         

            ContentValues values = new ContentValues();

                values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
                values.Put(MediaStore.IMediaColumns.MimeType, "application/octet-stream");
               // values.Put(MediaStore.IMediaColumns.RelativePath, "Download/" + file);
                values.Put(MediaStore.IMediaColumns.RelativePath, "Download/"+
                    StaticVariables.core.GetAppInfo().AppName + 
                    "/Docs/" 
                    + targetSubFolder);


                var uri = context.ContentResolver.Insert(
                    MediaStore.Downloads.ExternalContentUri,
                    values);

                

                using var stream = context.ContentResolver.OpenOutputStream(uri);
                stream!.Write(data, 0, data.Length);
            }


        }
    


}
#endif
