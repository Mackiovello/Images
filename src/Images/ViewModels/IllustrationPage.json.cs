using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Simplified.Ring1;
using Starcounter;

namespace Images {
    partial class IllustrationPage : Page, IBound<Simplified.Ring1.Illustration> {
        protected List<string> oldImageUrls = new List<string>();
        protected IllustrationHelper helper = new IllustrationHelper();

        protected override void OnData() {
            base.OnData();

            this.MaxFileSize = UploadHandlers.MaxFileSize;
            this.AllowedMimeTypes = string.Join(",", UploadHandlers.AllowedMimeTypes);

            if (this.Data == null) {
                Illustration i = new Illustration();

                i.Concept = new Something() { Name = "Standalone image" };
                i.Content = new Content();

                this.Data = i;
            }
        }

        void Handle(Input.Delete Action) {
            this.ParentPage.ConfirmAction = () => {
                Db.Transact(() => {
                    this.helper.DeleteFile(this.Data);

                    if (this.Data.Content != null) {
                        this.Data.Content.Delete();
                    }

                    this.Data.Delete();
                });
            };

            if (this.Data.Concept != null) {
                this.ParentPage.Confirm.Message = "Are you sure want to delete image [" + this.Data.Concept.Name + "]?";
            } else {
                this.ParentPage.Confirm.Message = "Are you sure want to delete this image?";
            }
        }

        void Handle(Input.Name Action) {
            this.Data.Concept = Db.SQL<Simplified.Ring1.Something>("SELECT o FROM Simplified.Ring1.Something o WHERE Name = ?", Action.Value).First;
        }

        void Handle(Input.Cancel Action) {
            this.RedirectUrl = "/images";
        }

        void Handle(Input.Save Action) {
            foreach (string s in this.oldImageUrls) {
                this.helper.DeleteFile(s);
            }
            
            this.Transaction.Commit();
            this.RedirectUrl = "/images";
        }

        void Handle(Input.ImageURL Action) {
            oldImageUrls.Add(Action.OldValue);
        }

        IllustrationsPage ParentPage {
            get {
                return this.Parent.Parent as IllustrationsPage;
            }
        }
    }
}
