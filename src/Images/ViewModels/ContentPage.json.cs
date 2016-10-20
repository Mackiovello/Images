using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper helper = new IllustrationHelper();
        public bool IsVideo
        {
            get
            {
                return helper.IsVideo(this.MimeType);
            }
        }

        public bool IsImage
        {
            get
            {
                return helper.IsImage(this.MimeType);
            }
        }
    }
}
