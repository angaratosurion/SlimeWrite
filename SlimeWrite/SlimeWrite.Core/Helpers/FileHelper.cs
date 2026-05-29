using System;
using System.Collections.Generic;
using System.Text;

namespace SlimeWrite.Core.Helpers
{
    public static class FileHelper
    {
        public static bool IsPlainTextOnly(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            const int sampleSize = 8192;
            byte[] buffer = new byte[sampleSize];

            using FileStream fs = File.OpenRead(filePath);
            int bytesRead = fs.Read(buffer, 0, buffer.Length);

            if (bytesRead == 0)
                return true;

            byte[] data = buffer[..bytesRead];

            // Binary/container signatures
            if (IsKnownBinaryFormat(data))
                return false;

            // Null bytes = binary
            if (ContainsNullBytes(data))
                return false;

            string text;

            // UTF detection + strict validation
            if (HasUtf8Bom(data))
            {
                if (!TryDecode(data[3..],
                    new UTF8Encoding(false, true),
                    out text))
                    return false;
            }
            else if (HasUtf16LeBom(data))
            {
                if (!TryDecode(data[2..],
                    new UnicodeEncoding(false, true, true),
                    out text))
                    return false;
            }
            else if (HasUtf16BeBom(data))
            {
                if (!TryDecode(data[2..],
                    new UnicodeEncoding(true, true, true),
                    out text))
                    return false;
            }
            else
            {
                // Strict UTF-8 only
                if (!TryDecode(data,
                    new UTF8Encoding(false, true),
                    out text))
                    return false;
            }

            text = text.TrimStart();

            // Exclude HTML / XML
            if (LooksLikeHtml(text) || LooksLikeXml(text))
                return false;

            return true;
        }

        private static bool TryDecode(
            byte[] data,
            Encoding encoding,
            out string text)
        {
            try
            {
                text = encoding.GetString(data);
                return true;
            }
            catch (DecoderFallbackException)
            {
                text = "";
                return false;
            }
        }

        private static bool ContainsNullBytes(byte[] data)
        {
            foreach (byte b in data)
            {
                if (b == 0)
                    return true;
            }
            return false;
        }

        private static bool LooksLikeHtml(string text)
        {
            string t = text.ToLowerInvariant();

            return t.StartsWith("<!doctype html") ||
                   t.StartsWith("<html") ||
                   t.Contains("<body") ||
                   t.Contains("<head");
        }

        private static bool LooksLikeXml(string text)
        {
            string t = text.ToLowerInvariant();

            return t.StartsWith("<?xml") ||
                   (t.StartsWith("<") &&
                    t.Contains(">") &&
                    t.Contains("</"));
        }

        private static bool IsKnownBinaryFormat(byte[] data)
        {
            // ZIP / DOCX / XLSX / PPTX / ODT
            if (data.Length >= 4 &&
                data[0] == 0x50 &&
                data[1] == 0x4B)
                return true;

            // PDF
            if (data.Length >= 4 &&
                data[0] == 0x25 &&
                data[1] == 0x50 &&
                data[2] == 0x44 &&
                data[3] == 0x46)
                return true;

            // OLE Office (.doc .xls .ppt)
            if (data.Length >= 8 &&
                data[0] == 0xD0 &&
                data[1] == 0xCF &&
                data[2] == 0x11 &&
                data[3] == 0xE0)
                return true;

            return false;
        }

        private static bool HasUtf8Bom(byte[] data)
        {
            return data.Length >= 3 &&
                   data[0] == 0xEF &&
                   data[1] == 0xBB &&
                   data[2] == 0xBF;
        }

        private static bool HasUtf16LeBom(byte[] data)
        {
            return data.Length >= 2 &&
                   data[0] == 0xFF &&
                   data[1] == 0xFE;
        }

        private static bool HasUtf16BeBom(byte[] data)
        {
            return data.Length >= 2 &&
                   data[0] == 0xFE &&
                   data[1] == 0xFF;
        }
    }
}
