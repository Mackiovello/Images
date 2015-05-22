﻿using System;
using System.IO;
using Starcounter;
using Simplified.Ring1;

namespace Images {
    public class IllustrationHelper {
        public string GetUploadDirectory() {
            return System.IO.Path.Combine(Starcounter.Application.Current.WorkingDirectory, "media");
        }

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
