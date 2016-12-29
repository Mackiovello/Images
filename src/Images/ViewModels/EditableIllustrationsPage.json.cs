using System.Collections.Generic;
using System.Linq;
using Simplified.Ring1;
using Starcounter;

namespace Images
{
    partial class EditableIllustrationsPage : Json, IBound<Something>
    {
        static EditableIllustrationsPage()
        {
            DefaultTemplate.Illustrations.Bind = nameof(IllustrationsBind);
        }

        protected IllustrationHelper helper = new IllustrationHelper();
        public IEnumerable<Illustration> IllustrationsBind => Illustration.GetIllustrations(this.Data);

        protected override void OnData()
        {
            base.OnData();
            this.MaxFileSize = helper.GetMaximumFileSizeBytes();

            foreach (string s in UploadHandlers.AllowedMimeTypes)
            {
                this.AllowedMimeTypes.Add().StringValue = s;
            }

            this.Illustrations = Illustration.GetIllustrations(this.Data);
            this.Selected.Data = this.Illustrations.FirstOrDefault()?.Data;

            this.SessionId = Session.Current.SessionId;
        }

        void Handle(Input.Delete action)
        {
            if (this.Selected == null || Selected.Data == null)
            {
                return;
            }

            Illustration selected = Selected.Data;
            this.Selected.Data = null;

            this.helper.DeleteFile(selected);
            selected.Content?.Delete();
            selected.Delete();
            this.Transaction.Commit();

            this.Selected.Data = Illustrations.FirstOrDefault()?.Data;// ?? new Illustration() { Concept = this.Data, Content = new Content() };
        }

        void Handle(Input.Save action)
        {
            if (Selected.Data == null && Selected.Content.Data == null)
            {
                Selected.Data = new Illustration()
                {
                    Concept = this.Data,
                    Content = new Content()
                    {
                        Name = Selected.Content.Name,
                        MimeType = Selected.Content.MimeType,
                        URL = Selected.Content.URL
                    }
                };
            }

            Selected.Data.Name = Selected.Content.Name;
            Selected.Data.Concept = this.Data;
            Transaction.Commit();
        }

        void Handle(Input.Add action)
        {
            Illustration empty = Illustrations.FirstOrDefault(val => string.IsNullOrEmpty(val.Data.Content.URL))?.Data;
            Selected.Data = empty ?? new Illustration() { Concept = this.Data, Content = new Content() };
            Selected.Data.Concept = this.Data;
        }

        [EditableIllustrationsPage_json.Selected]
        partial class IllustrationSelectedPage : Json, IBound<Simplified.Ring1.Illustration>
        {
        }

        [EditableIllustrationsPage_json.Illustrations]
        partial class IllustrationItemPage : Json, IBound<Simplified.Ring1.Illustration>
        {
            EditableIllustrationsPage ParentPage => Parent.Parent as EditableIllustrationsPage;

            public string PreviewURL
            {
                get
                {
                    if (ParentPage.helper.IsVideo(Data.Content.MimeType))
                    {
                        return "/images/css/video_preview.png";
                    }
                    else if (ParentPage.helper.IsImage(Data.Content.MimeType))
                    {
                        return Data.Content.URL;
                    }
                    else if (!string.IsNullOrEmpty(Data.Content.URL))
                    {
                        return "/images/css/file_preview.png";
                    }
                    else
                    {
                        return "/images/css/empty_preview.png";
                    }
                }
            }

            void Handle(Input.Select action)
            {
                ParentPage.Selected.Data = this.Data;
            }
        }
    }
}
