using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper Helper = new IllustrationHelper();
        public bool IsVideo => Helper.IsVideo(MimeType);
        public bool IsImage => Helper.IsImage(MimeType);
    }
}
