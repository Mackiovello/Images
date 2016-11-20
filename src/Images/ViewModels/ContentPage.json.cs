using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper helper = new IllustrationHelper();
        public bool IsVideo => helper.IsVideo(MimeType);
        public bool IsImage => helper.IsImage(MimeType);
    }
}
