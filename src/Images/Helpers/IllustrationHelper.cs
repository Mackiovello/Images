using System;
using System.IO;
using Starcounter;
using Simplified.Ring1;

namespace Images {
    public class IllustrationHelper {

        public string UploadFolderName = "media";

        public string GetSharedFolder() {

            string rootPath = System.IO.Path.GetPathRoot(Starcounter.Application.Current.FilePath);
            return System.IO.Path.Combine(rootPath, "UploadedFiles");
        }

        public string GetUploadDirectory() {

            return Path.Combine(this.GetSharedFolder(), this.UploadFolderName);
        }

        public void DeleteFile(Illustration Illustration) {
            if (Illustration.Concept == null) {
                return;
            }

            FileInfo fi = new FileInfo(this.GetSharedFolder() + Illustration.Content.URL);

            if (fi.Exists) {
                fi.Delete();
            }
        }

        public void DeleteFile(string ImageUrl) {
            if (string.IsNullOrEmpty(ImageUrl)) {
                return;
            }

            FileInfo fi = new FileInfo(this.GetSharedFolder() + ImageUrl);

            if (fi.Exists) {
                fi.Delete();
            }
        }

        public void DeleteOldFiles() {
            string filePath = this.GetUploadDirectory();
            DirectoryInfo di = new DirectoryInfo(filePath);

            if (!di.Exists) {
                return;
            }

            foreach (FileInfo fi in di.GetFiles()) {
                Content img = Db.SQL<Content>("SELECT c FROM Simplified.Ring1.Content c WHERE c.URL LIKE ?", "%" + fi.Name).First;

                if (img == null) {
                    fi.Delete();
                }
            }
        }
    }
}
