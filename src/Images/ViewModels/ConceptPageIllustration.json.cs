using Starcounter;
using Simplified.Ring1;

namespace Images {
    partial class ConceptPageIllustration : Page, IBound<Illustration> {
        protected string oldImageUrl = null;
        protected IllustrationHelper helper = new IllustrationHelper();

        protected override void OnData() {
            base.OnData();

            this.MaxFileSize = UploadHandlers.MaxFileSize;

            foreach (string s in UploadHandlers.AllowedMimeTypes) {
                this.AllowedMimeTypes.Add().StringValue = s;
            }
        }

        public void RefreshRelation(string objectId)
        {
            var il = (Illustration)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
            Data = il;
        }

        public string URL {
            get {
                if (this.Data != null && Data.Content != null) {
                    return this.Data.Content.URL;
                }

                return null;
            }
            set {
                if (Data.Content == null)
                {
                    Data.Content = new Content
                    {
                        URL = value
                    };
                }
                Data.Name = value;
                oldImageUrl = Data.Content.URL;
            }
        }

        void Handle(Input.Delete action) {
            if (this.Data != null) {
                this.helper.DeleteFile(this.Data);

                if (this.Data.Content != null) {
                    this.Data.Content.Delete();
                }

                this.Data.Delete();
            }
        }

        void Handle(Input.Save action) {
            this.Transaction.Commit();
        }
    }
}
