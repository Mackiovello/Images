using System.Linq;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Authorization.Attributes;
using Starcounter.Authorization.Routing;

namespace Images
{
    [PartialUrl("/images/partials/settings")]
    [RequirePermission(typeof(OpenBasicPages))]
    partial class SettingsPage : Json, IBound<ImagesSettings>
    {
        private readonly IllustrationHelper _illustrationHelper = new IllustrationHelper();

        [UriToContext]
        public static ImagesSettings CreateContext(string[] args)
        {
            return Db.SQL<ImagesSettings>("SELECT s FROM Simplified.Ring6.ImagesSettings s").FirstOrDefault();
        }

        public decimal MaximumFileSizeMiB
        {
            get { return _illustrationHelper.BytesToMiB(Data.MaximumFileSize); }
            set { Data.MaximumFileSize = _illustrationHelper.MiBToBytes(value); }
        }

        void Handle(Input.Save action)
        {
            Transaction.Commit();
            UploadHandlers.ReloadHelperPath();
        }

        void Handle(Input.CleanUpFiles action)
        {
            _illustrationHelper.DeleteOldFiles();
            Transaction.Commit();
        }
    }
}
