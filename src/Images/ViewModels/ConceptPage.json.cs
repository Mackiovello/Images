using System.IO;
using Starcounter;
using Simplified.Ring1;

namespace Images {
    partial class ConceptPage : Page, IBound<Something> {
        protected string oldImageUrl = null;
        protected IllustrationHelper helper = new IllustrationHelper();

        protected override void OnData() {
            base.OnData();

            this.MaxFileSize = helper.GetMaximumFileSizeBytes();

            foreach (string s in UploadHandlers.AllowedMimeTypes) {
                this.AllowedMimeTypes.Add().StringValue = s;
            }

            this.SessionId = Session.Current.SessionId;
        }

        public string URL {
            get {
                return this.Data.Illustration?.Content?.URL;
            }
            set {
                this.helper.DeleteFile(this.oldImageUrl);

                Illustration illustration = this.Data.Illustration;

                if (illustration == null) {
                    illustration = new Illustration {Concept = this.Data};
                }

                if (illustration.Content == null)
                {
                    illustration.Content = new Content()
                    {
                        URL = value,
                        Path = helper.GetUploadDirectoryWithRoot().Replace("/", "\\"),
                        Name = Path.GetFileName(value)
                    };
                }

                illustration.Content.URL = value;
                illustration.Name = value;

                oldImageUrl = illustration.Content.URL;
            }
        }

        void Handle(Input.Delete action) {
            if (this.Data.Illustration == null) return;

            this.helper.DeleteFile(this.Data.Illustration);
            this.Data.Illustration.Content?.Delete();
        }

        void Handle(Input.Save action) {
            this.Transaction.Commit();
        }
    }
}
