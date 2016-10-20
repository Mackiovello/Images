using System;
using System.Linq;
using Simplified.Ring1;
using Starcounter;

namespace Images
{
    partial class IllustrationsPage : Json, IBound<Something>
    {
        protected IllustrationHelper helper = new IllustrationHelper();
        protected override void OnData()
        {
            base.OnData();
            this.Illustrations = Illustration.GetIllustrations(this.Data);
            this.Selected.Data = this.Illustrations.FirstOrDefault()?.Data;
        }

        //public Action ConfirmAction = null;

        //public QueryResultRows<Illustration> GetIllustrations => Db.SQL<Illustration>("SELECT o FROM Simplified.Ring1.Illustration o");

        //static IllustrationsPage()
        //{
        //    DefaultTemplate.Illustrations.Bind = nameof(GetIllustrations);
        //}

        //void Handle(Input.Add action)
        //{
        //    this.RedirectUrl = "images/image/";
        //}

        //[IllustrationsPage_json.Confirm]
        //partial class PersonsConfirmPage : Page
        //{
        //    void Cancel()
        //    {
        //        this.ParentPage.Confirm.Message = null;
        //        this.ParentPage.ConfirmAction = null;
        //    }

        //    void Handle(Input.Reject Action)
        //    {
        //        Cancel();
        //    }

        //    void Handle(Input.Ok Action)
        //    {
        //        if (this.ParentPage.ConfirmAction != null)
        //        {
        //            this.ParentPage.ConfirmAction();
        //        }

        //        Cancel();
        //    }

        //    public IllustrationsPage ParentPage
        //    {
        //        get
        //        {
        //            return this.Parent as IllustrationsPage;
        //        }
        //    }
        //}

        [IllustrationsPage_json.Illustrations]
        partial class IllustrationItemPage : Json, IBound<Simplified.Ring1.Illustration>
        {
            IllustrationsPage ParentPage => Parent.Parent as IllustrationsPage;

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
