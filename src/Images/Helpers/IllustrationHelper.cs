using System;
using System.IO;
using Starcounter;
using Simplified.Ring1;

namespace Images {
    public class IllustrationHelper {
        public void DeleteFile(Illustration Illustration) {
            if (Illustration.Concept == null) {
                return;
            }

            FileInfo fi = new FileInfo(Application.Current.WorkingDirectory + Illustration.Content.URL);

            if (fi.Exists) {
                fi.Delete();
            }
        }

        public void DeleteFile(string ImageUrl) {
            if (string.IsNullOrEmpty(ImageUrl)) {
                return;
            }

            FileInfo fi = new FileInfo(Application.Current.WorkingDirectory + ImageUrl);

            if (fi.Exists) {
                fi.Delete();
            }
        }
    }
}
