using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;
using System.Linq;

namespace Images
{
    partial class ConceptPage : Json, IBound<Something>
    {
        static ConceptPage()
        {
            DefaultTemplate.Illustrations.Bind = nameof(ConceptIllustrations);

            DefaultTemplate.Selected.MimeType.Bind = nameof(IllustrationPage.ContentMimeType);
            DefaultTemplate.Selected.IsVideo.Bind = nameof(IllustrationPage.IsVideoBind);
            DefaultTemplate.Selected.IsImage.Bind = nameof(IllustrationPage.IsImageBind);
            DefaultTemplate.Selected.Name.Bind = nameof(IllustrationPage.ConceptName);
            DefaultTemplate.Selected.ImageURL.Bind = nameof(IllustrationPage.ContentURL);
        }
        
        protected IllustrationHelper helper = new IllustrationHelper();

        public IEnumerable<Illustration> ConceptIllustrations => Illustration.GetIllustrations(this.Data);

        protected override void OnData()
        {
            base.OnData();

            this.MaxFileSize = helper.GetMaximumFileSize();

            foreach (string s in UploadHandlers.AllowedMimeTypes)
            {
                this.AllowedMimeTypes.Add().StringValue = s;
            }

            this.Selected.Data = ConceptIllustrations.FirstOrDefault() ?? new Illustration() { Concept = this.Data, Content = new Content() };
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
            Illustration empty = ConceptIllustrations.FirstOrDefault(val => string.IsNullOrEmpty(val.Content.URL));
            Selected.Data = empty ?? new Illustration() { Concept = this.Data, Content = new Content() };
        }

        [ConceptPage_json.Illustrations]
        partial class IllustrationItemPage : Json, IBound<Simplified.Ring1.Illustration>
        {
            static IllustrationItemPage()
            {
                DefaultTemplate.IsVideo.Bind = nameof(IsVideoBind);
                DefaultTemplate.IsImage.Bind = nameof(IsImageBind);
                DefaultTemplate.Name.Bind = nameof(ConceptName);
                DefaultTemplate.MimeType.Bind = nameof(ContentMimeType);
                DefaultTemplate.ImageURL.Bind = nameof(ContentURL);
                DefaultTemplate.PreviewURL.Bind = nameof(PreviewURLBind);
            }

            ConceptPage ParentPage => Parent.Parent as ConceptPage;
            public bool IsImageBind => ParentPage.helper.IsImage(Data?.Content?.MimeType);
            public bool IsVideoBind => ParentPage.helper.IsVideo(Data?.Content?.MimeType);
            public string PreviewURLBind
            {
                get
                {
                    if (IsVideoBind)
                    {
                        return "/images/css/video_preview.png";
                    }
                    else if (IsImageBind)
                    {
                        return ContentURL;
                    }
                    else if (!string.IsNullOrEmpty(ContentURL))
                    {
                        return "/images/css/file_preview.png";
                    }
                    else
                    {
                        return "/images/css/empty_preview.png";
                    }
                }
            }

            public string ConceptName
            {
                get
                {
                    return Data?.Concept?.Name;
                }
            }
            public string ContentMimeType
            {
                get
                {
                    return Data?.Content?.MimeType;
                }
            }

            public string ContentURL
            {
                get
                {
                    return Data?.Content?.URL;
                }
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
