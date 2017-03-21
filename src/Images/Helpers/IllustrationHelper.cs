using System.IO;
using System.Linq;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring6;
using System;
using System.Web;
using System.Collections.Generic;

namespace Images
{
    public class IllustrationHelper
    {
        private readonly string _rootPath;
        private readonly ImagesSettings _imagesSettings;
        public static readonly int BytesInMiB = 1024 * 1024;
        public static readonly int DefaultMaximumFileSize = 10 * BytesInMiB;

        public IllustrationHelper()
        {
            _imagesSettings = Db.SQL("SELECT s FROM Simplified.Ring6.ImagesSettings s").First as ImagesSettings;

            if (_imagesSettings == null)
            {
                Db.Transact(() =>
                {
                    new ImagesSettings
                    {
                        MaximumFileSize = GetMaximumFileSizeBytes(),
                        UploadFolderPath = GetUploadDirectory()
                    };
                });

                _imagesSettings = Db.SQL("SELECT s FROM Simplified.Ring6.ImagesSettings s").First as ImagesSettings;
            }

            var rootPath = Path.GetPathRoot(Application.Current.FilePath);
            _rootPath = Path.Combine(rootPath, "UploadedFiles");
        }

        public bool IsVideo(string mimeType) {
            return mimeType != null && mimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsImage(string mimeType) {
            return mimeType != null && mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        }

        public string GetUploadDirectory()
        {
            string path;
            if (_imagesSettings == null)
            {
                path = "/";
            }
            else
            {
                path = _imagesSettings.UploadFolderPath;
            }

            return path;
        }

        public string GetUploadRoot()
        {
            return _rootPath;
        }

        public string GetUploadDirectoryWithRoot()
        {
            return GetUploadRoot() + GetUploadDirectory();
        }

        public decimal BytesToMiB(int size)
        {
            return (decimal)size / BytesInMiB;
        }

        public int MiBToBytes(decimal size)
        {
            return (int)(size * BytesInMiB);
        }

        public int GetMaximumFileSizeBytes()
        {
            return _imagesSettings?.MaximumFileSize ?? DefaultMaximumFileSize;
        }

        public void DeleteFile(Illustration illustration)
        {
            if (illustration.Concept == null)
            {
                return;
            }

            var fi = new FileInfo(GetUploadRoot() + illustration.Content.URL);

            if (fi.Exists)
            {
                fi.Delete();
            }
        }

        public void DeleteFile(Content content)
        {
            var fi = new FileInfo(GetUploadRoot() + content.URL);

            if (fi.Exists)
            {
                fi.Delete();
            }
        }

        public void DeleteFile(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return;
            }

            var fi = new FileInfo(GetUploadRoot() + imageUrl);

            if (fi.Exists)
            {
                fi.Delete();
            }
        }

        public void DeleteOldFiles()
        {
            var filePath = GetUploadDirectoryWithRoot();
            var di = new DirectoryInfo(filePath);

            if (!di.Exists)
            {
                return;
            }

            foreach (
                var fi in from fi in di.GetFiles()
                let img = Db.SQL("SELECT c FROM Simplified.Ring1.Content c WHERE c.URL LIKE ?", "%" + fi.Name).First as Content
                where img == null
                select fi)
            {
                fi.Delete();
            }
        }

        public void GuessMimeTypeForContents(IEnumerable<Content> contents)
        {
            foreach (var content in contents.Where(e => !string.IsNullOrEmpty(e.URL)))
            {
                content.MimeType = MimeMapping.GetMimeMapping(content.URL);
            }
        }
    }
}
