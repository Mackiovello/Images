using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;
using System.Linq;

namespace Images
{
    partial class ConceptPage : Page, IBound<Something>
    {
        protected string oldImageUrl = null;
        protected IllustrationHelper helper = new IllustrationHelper();

        static ConceptPage()
        {
            //DefaultTemplate.ImageURL.Bind = nameof(URL);
            //DefaultTemplate.ImageMimeType.Bind = nameof(MimeType);
            DefaultTemplate.Illustrations.Bind = nameof(ConceptIllustrations);

            DefaultTemplate.Selected.MimeType.Bind = nameof(IllustrationPage.ContentMimeType);
            DefaultTemplate.Selected.IsVideo.Bind = nameof(IllustrationPage.GetIsVideo);
            DefaultTemplate.Selected.IsImage.Bind = nameof(IllustrationPage.GetIsImage);
            DefaultTemplate.Selected.Name.Bind = nameof(IllustrationPage.ConceptName);
            DefaultTemplate.Selected.MimeType.Bind = nameof(IllustrationPage.ContentMimeType);
            DefaultTemplate.Selected.ImageURL.Bind = nameof(IllustrationPage.ContentURL);
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

        public IEnumerable<Illustration> ConceptIllustrations => Illustration.GetIllustrations(this.Data);

        protected override void OnData()
        {
            base.OnData();

            this.MaxFileSize = helper.GetMaximumFileSize();

            foreach (string s in UploadHandlers.AllowedMimeTypes)
            {
                this.AllowedMimeTypes.Add().StringValue = s;
            }
            
            //this.SessionId = Session.Current.SessionId;
            this.Selected.Data = ConceptIllustrations.FirstOrDefault() ?? new Illustration() { Concept = this.Data, Content = new Content() }; ;
        }

        void Handle(Input.Delete action)
        {
            if (this.Selected == null)
            {
                return;
            }

            this.helper.DeleteFile(Selected.Data);
            Selected.Data.Content?.Delete();
            Selected.Data.Delete();
            this.Transaction.Commit();

            this.Selected.Data = ConceptIllustrations.FirstOrDefault() ?? new Illustration() { Concept = this.Data, Content = new Content() };
        }

        void Handle(Input.Save action)
        {
            this.Transaction.Commit();
        }

        void Handle(Input.Add action)
        {
            Selected.Data = new Illustration() { Concept = this.Data, Content = new Content() };
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

        [ConceptPage_json.Illustrations]
        partial class IllustrationPage : Page, IBound<Simplified.Ring1.Illustration>
        {
            public ConceptPage ParentPage => Parent.Parent as ConceptPage;
            public bool GetIsImage => ParentPage.helper.IsImage(Data?.Content?.MimeType);
            public bool GetIsVideo => ParentPage.helper.IsVideo(Data?.Content?.MimeType);

            public string ConceptName
            {
                get
                {
                    return Data?.Concept?.Name;
                }
                set
                {
                    Data.Concept.Name = value;
                }
            }
            public string ContentMimeType
            {
                get
                {
                    return Data?.Content?.MimeType;
                }
                set
                {
                    Data.Content.MimeType = value;
                }
            }

            public string ContentURL
            {
                get
                {
                    return Data?.Content?.URL;
                }
                set
                {
                    Data.Content.URL = value;
                }
            }

            static IllustrationPage()
            {
                DefaultTemplate.IsVideo.Bind = nameof(GetIsVideo);
                DefaultTemplate.IsImage.Bind = nameof(GetIsImage);
                DefaultTemplate.Name.Bind = nameof(ConceptName);
                DefaultTemplate.MimeType.Bind = nameof(ContentMimeType);
                DefaultTemplate.ImageURL.Bind = nameof(ContentURL);
            }

            protected override void OnData()
            {
                this.Url = string.Format("/images/image/{0}", this.Key);
            }

            void Handle(Input.Select action)
            {
                ParentPage.Selected.Data = this.Data;
            }
        }
    }
}
