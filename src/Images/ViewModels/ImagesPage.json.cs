using System;
using Simplified.Ring1;
using Starcounter;

namespace Images
{
    partial class ImagesPage : Json
    {
        protected IllustrationHelper Helper = new IllustrationHelper();
        public Action ConfirmAction;

        public QueryResultRows<Content> GetImages => Db.SQL<Content>("SELECT o FROM Simplified.Ring1.Content o");

        static ImagesPage()
        {
            DefaultTemplate.Images.Bind = nameof(GetImages);
        }

        void Handle(Input.Add action)
        {
            RedirectUrl = "images/image";
        }

        [ImagesPage_json.Confirm]
        partial class ImagesConfirmPage : Page
        {
            void Cancel()
            {
                ParentPage.Confirm.Message = null;
                ParentPage.ConfirmAction = null;
            }

            void Handle(Input.Reject action)
            {
                Cancel();
            }

            void Handle(Input.Ok action)
            {
                ParentPage.ConfirmAction?.Invoke();
                Cancel();
            }

            public ImagesPage ParentPage => this.Parent as ImagesPage;
        }

        [ImagesPage_json.Images]
        partial class ImageItemPage : Json, IBound<Content>
        {
            ImagesPage ParentPage => Parent.Parent as ImagesPage;

            public string PreviewURL
            {
                get
                {
                    var empty = "/images/css/empty_preview.png";
                    if (Data == null)
                    {
                        return empty;
                    }
                    if (ParentPage.Helper.IsVideo(Data.MimeType))
                    {
                        return "/images/css/video_preview.png";
                    }
                    if (ParentPage.Helper.IsImage(Data.MimeType))
                    {
                        return Data.URL;
                    }
                    if (!string.IsNullOrEmpty(Data.URL))
                    {
                        return "/images/css/file_preview.png";
                    }
                    return empty;
                }
            }

            public string ContentURL => $"/images/contents/{Data.Key}";
            public string ContentEditURL => $"/images/image/{Data.Key}";

            void Handle(Input.Delete action)
            {
                ParentPage.ConfirmAction = () =>
                {
                    Db.Transact(() =>
                    {
                        ParentPage.Helper.DeleteFile(this.Data);
                        var illustrations = Db.SQL<Illustration>("SELECT o FROM Simplified.Ring1.Illustration o WHERE o.Content = ?", this.Data);
                        foreach (Illustration item in illustrations)
                        {
                            item.Delete();
                        }
                        Data.Delete();
                    });
                };

                ParentPage.Confirm.Message = $"Are you sure want to delete image [{Data.Name}]?";
            }
        }
    }
}
