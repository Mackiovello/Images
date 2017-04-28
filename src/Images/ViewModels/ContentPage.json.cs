using Starcounter;
using Simplified.Ring1;

namespace Images
{
    [PartialUrl("/images/partials/contents/{?}")]
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
