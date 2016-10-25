using System;
using Simplified.Ring1;
using Starcounter;

namespace Images
{
    partial class ImagesPage : Json
    {
        protected IllustrationHelper helper = new IllustrationHelper();
        public Action ConfirmAction = null;

        public QueryResultRows<Content> GetImages => Db.SQL<Content>("SELECT o FROM Simplified.Ring1.Content o");

        static ImagesPage()
        {
            DefaultTemplate.Images.Bind = nameof(GetImages);
        }

        void Handle(Input.Add action)
        {
            this.RedirectUrl = "images/image";
        }

        [ImagesPage_json.Confirm]
        partial class ImagesConfirmPage : Page
        {
            void Cancel()
            {
                this.ParentPage.Confirm.Message = null;
                this.ParentPage.ConfirmAction = null;
            }

            void Handle(Input.Reject Action)
            {
                Cancel();
            }

            void Handle(Input.Ok Action)
            {
                if (this.ParentPage.ConfirmAction != null)
                {
                    this.ParentPage.ConfirmAction();
                }

                Cancel();
            }

            public ImagesPage ParentPage
            {
                get
                {
                    return this.Parent as ImagesPage;
                }
            }
        }

        [ImagesPage_json.Images]
        partial class ImageItemPage : Json, IBound<Simplified.Ring1.Content>
        {
            ImagesPage ParentPage => Parent.Parent as ImagesPage;

            public string PreviewURL
            {
                get
                {
                    string empty = "/images/css/empty_preview.png";
                    if (Data == null)
                    {
                        return empty;
                    }

                    if (ParentPage.helper.IsVideo(Data.MimeType))
                    {
                        return "/images/css/video_preview.png";
                    }
                    else if (ParentPage.helper.IsImage(Data.MimeType))
                    {
                        return Data.URL;
                    }
                    else if (!string.IsNullOrEmpty(Data.URL))
                    {
                        return "/images/css/file_preview.png";
                    }
                    return empty;
                }
            }

            public string ContentURL
            {
                get
                {

                    return string.Format("/images/content/{0}", this.Data.Key);
                }
            }

            public string ContentEditURL
            {
                get
                {

                    return string.Format("/images/content-edit/{0}", this.Data.Key);
                }
            }

            void Handle(Input.Delete Action)
            {
                this.ParentPage.ConfirmAction = () =>
                {
                    Db.Transact(() =>
                    {
                        this.ParentPage.helper.DeleteFile(this.Data);
                        var illustrations = Db.SQL<Illustration>("SELECT o FROM Simplified.Ring1.Illustration o WHERE o.Content = ?", this.Data);
                        foreach (Illustration item in illustrations)
                        {
                            item.Delete();
                        }
                        this.Data.Delete();
                    });
                };

                this.ParentPage.Confirm.Message = "Are you sure want to delete image [" + this.Data.Name + "]?";
            }
        }
    }
}
