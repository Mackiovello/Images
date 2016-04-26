using Images.Helpers;
using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ConceptIllustrationWarningPage : Page
    {
        public void RefreshData(string chatMessageTextId)
        {
            var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(chatMessageTextId)) as Illustration;
            Warning = ImageValidator.IsValid(illustration);
        }
    }
}
