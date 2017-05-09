using Simplified.Ring1;
using Starcounter;
using Starcounter.Authorization.Attributes;

namespace Images
{
    [PartialUrl("/images/partials/contents/{?}")]
    [Routing(typeof(ContenPageRouting))]
    [RequirePermission(typeof(OpenBasicPages))]
    partial class ContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper Helper = new IllustrationHelper();
        public bool IsVideo => Helper.IsVideo(MimeType);
        public bool IsImage => Helper.IsImage(MimeType);

        public void Handle(Input.ShowLightBox action)
        {
            IsLightBoxVisible = true;
        }
    }
}
