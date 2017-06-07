using System.Linq;
using Simplified.Ring1;
using Starcounter;
using Starcounter.Authorization.Attributes;
using Starcounter.Authorization.Routing.Middleware;

namespace Images
{
    [PartialUrl("/images/partials/somethings/{?}")]
    [UseDbScope(false)]
    [RequirePermission(typeof(OpenBasicPages))]
    partial class IllustrationsPage : Json, IBound<Something>
    {
        private IllustrationHelper helper = new IllustrationHelper();

        protected override void OnData()
        {
            base.OnData();
            this.Illustrations = Illustration.GetIllustrations(this.Data);
            this.Selected.Data = this.Illustrations.FirstOrDefault()?.Data;
        }

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
