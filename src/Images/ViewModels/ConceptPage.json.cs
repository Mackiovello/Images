using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ConceptPage : Page, IBound<Something>
    {
        protected string oldImageUrl = null;
        protected IllustrationHelper helper = new IllustrationHelper();

        static ConceptPage()
        {
            DefaultTemplate.ImageURL.Bind = nameof(URL);
            DefaultTemplate.ImageMimeType.Bind = nameof(MimeType);
        }

        public string MimeType
        {
            get
            {
                return this.Data.Illustration?.Content?.MimeType;
            }
            set
            {
                Illustration illustration = InitIllustration();
                illustration.Content.MimeType = value;
            }
        }

        public string URL
        {
            get
            {
                return this.Data.Illustration?.Content?.URL;
            }
            set
            {
                if (URL != value)
                {
                    oldImageUrl = URL;
                }

                Illustration illustration = InitIllustration();

                illustration.Content.URL = value;
                illustration.Name = value;

                oldImageUrl = illustration.Content.URL;
            }
        }

        protected override void OnData()
        {
            base.OnData();

            this.MaxFileSize = helper.GetMaximumFileSize();

            foreach (string s in UploadHandlers.AllowedMimeTypes)
            {
                this.AllowedMimeTypes.Add().StringValue = s;
            }

            this.SessionId = Session.Current.SessionId;
        }

        void Handle(Input.Delete action)
        {
            if (this.Data.Illustration == null)
            {
                return;
            }

            this.helper.DeleteFile(this.Data.Illustration);
            this.Data.Illustration.Content?.Delete();
        }

        void Handle(Input.Save action)
        {
            this.Transaction.Commit();
        }

        Illustration InitIllustration()
        {
            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                this.helper.DeleteFile(this.oldImageUrl);
                oldImageUrl = string.Empty;
            }

            Illustration illustration = this.Data.Illustration;

            if (illustration == null)
            {
                illustration = new Illustration { Concept = this.Data };
            }

            if (illustration.Content == null)
            {
                illustration.Content = new Content() { };
            }
            return illustration;
        }
    }
}
