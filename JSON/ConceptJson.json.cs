using Starcounter;

namespace Images.JSON {
    partial class ConceptJson : Page, IBound<Simplified.Ring1.Something> {
        protected override void OnData() {
            base.OnData();

            this.MaxFileSize = Image.ImageUpload.MaxFileSize;
            this.AllowedMimeTypes = string.Join(",", Image.ImageUpload.AllowedMimeTypes);
        }

        public string URL {

            get {

                if (this.Data.Illustration != null) {
                    return this.Data.Illustration.Content.URL;
                }
                return null;
            }
            set {

                Simplified.Ring1.Illustration illustration = this.Data.Illustration;
                if (illustration == null) {
                    illustration = new Simplified.Ring1.Illustration();
                    illustration.Content = new Simplified.Ring1.Content() { URL = value };
                    illustration.Concept = this.Data;
                } else {
                    illustration.Content.URL = value;
                }
            }
        }

        void Handle(Input.Delete action) {

            if (this.Data.Illustration != null) {
                if (this.Data.Illustration.Content != null) {
                    this.Data.Illustration.Content.Delete();
                }
                this.Data.Illustration.Delete();
            }
        }

        void Handle(Input.Save action) {
            this.Transaction.Commit();
        }
    }

}
