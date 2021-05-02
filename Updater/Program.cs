using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;

namespace Updater
{
    class Program
    {
        static void ExtractToDirectory(ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                string directory = Path.GetDirectoryName(completeFileName);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (file.Name != "" && file.Name != "Updater.exe")
                    file.ExtractToFile(completeFileName, true);
            }
        }

        static void Main(string[] args)
        {
            if(args.Length == 1)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.Headers.Add("User-Agent",
                        "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                        client.DownloadFile(args[0], "update.zip");
                        using (ZipArchive zipArchive = ZipFile.OpenRead("update.zip"))
                            ExtractToDirectory(zipArchive, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), true);
                        File.Delete("update.zip");
                        Process.Start("FloatTool.exe", "");
                    }
                }
                catch (Exception _) {}
            }
        }
    }
}
