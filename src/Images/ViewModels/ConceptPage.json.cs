using Starcounter;
using Simplified.Ring1;

namespace Images {
    partial class ConceptPage : Page, IBound<Something> {
        protected string oldImageUrl = null;
        protected IllustrationHelper helper = new IllustrationHelper();

        protected override void OnData() {
            base.OnData();

            this.MaxFileSize = helper.GetMaximumFileSize();

            foreach (string s in UploadHandlers.AllowedMimeTypes) {
                this.AllowedMimeTypes.Add().StringValue = s;
            }

            this.SessionId = Session.Current.SessionId;
        }

        public string URL {
            get {
                if (this.Data.Illustration != null) {
                    return this.Data.Illustration.Content.URL;
                }

                return null;
            }
            set {
                this.helper.DeleteFile(this.oldImageUrl);

                Illustration illustration = this.Data.Illustration;

                if (illustration == null) {
                    illustration = new Illustration();
                    illustration.Content = new Content() { URL = value };
                    illustration.Concept = this.Data;
                } else {
                    illustration.Content.URL = value;
                }

                illustration.Name = value;
                oldImageUrl = illustration.Content.URL;
            }
        }

        void Handle(Input.Delete action) {
            if (this.Data.Illustration != null) {
                this.helper.DeleteFile(this.Data.Illustration);

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
