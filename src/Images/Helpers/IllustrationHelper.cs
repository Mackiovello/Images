using System.IO;
using System.Linq;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring6;

namespace Images
{
    public class IllustrationHelper
    {
        public static string FolderName = "UploadedFiles";
        private readonly ImagesSettings _imagesSettings;

        public IllustrationHelper()
        {
            _imagesSettings = Db.SQL<ImagesSettings>("SELECT s FROM Simplified.Ring6.ImagesSettings s").First;
        }
        
        public string GetUploadDirectory()
        {
            string path;
            if (_imagesSettings == null)
            {
                var rootPath = Path.GetPathRoot(Application.Current.FilePath);
                path = Path.Combine(rootPath, FolderName);
            }
            else
            {
                path = _imagesSettings.UploadFolderPath;
            }

            return path;
        }

        public int GetMaximumFileSize()
        {
            return _imagesSettings?.MaximumFileSize ?? 1048576;
        }

        public void DeleteFile(Illustration illustration)
        {
            if (illustration.Concept == null)
            {
                return;
            }

            var fi = new FileInfo(GetUploadDirectory() + illustration.Content.URL);

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

            var fi = new FileInfo(GetUploadDirectory() + imageUrl);

            if (fi.Exists)
            {
                fi.Delete();
            }
        }

        public void DeleteOldFiles()
        {
            var filePath = GetUploadDirectory();
            var di = new DirectoryInfo(filePath);

            if (!di.Exists)
            {
                return;
            }

            foreach (
                var fi in from fi in di.GetFiles()
                let img = Db.SQL<Content>("SELECT c FROM Simplified.Ring1.Content c WHERE c.URL LIKE ?", "%" + fi.Name).First
                where img == null
                select fi)
            {
                fi.Delete();
            }
        }
    }
}
