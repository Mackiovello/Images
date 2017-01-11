using Simplified.Ring1;
using Starcounter;

namespace Images
{
    public class UpgradeData
    {
        /// <summary>
        /// In Images 3.0.6 and lower, the Content class did not include the image mime type information.
        /// It can be extrapolated from the file extension.
        /// </summary>
        public void UpgradeMimeType()
        {
            var contents = Db.SQL<Content>("SELECT c FROM Simplified.Ring1.Content c WHERE c.MimeType IS NULL");
            var illustrationHelper = new IllustrationHelper();
            Db.Transact(() =>
            {
                illustrationHelper.GuessMimeTypeForContents(contents);
            });
        }
    }
}
