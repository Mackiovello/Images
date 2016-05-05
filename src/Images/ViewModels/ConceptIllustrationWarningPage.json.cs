using Images.Helpers;
using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ConceptIllustrationWarningPage : Page
    {
        public void RefreshData(string contentId)
        {
            var content = DbHelper.FromID(DbHelper.Base64DecodeObjectID(contentId)) as Content;
            Warning = ImageValidator.IsValid(content);
        }
    }
}
