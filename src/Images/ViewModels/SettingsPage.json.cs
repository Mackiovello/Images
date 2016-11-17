using Starcounter;
using Simplified.Ring6;

namespace Images {
    partial class SettingsPage : Page, IBound<ImagesSettings> {
        public static IllustrationHelper IllustrationHelper = new IllustrationHelper();

        public void LoadDefaultData()
        {
            var settings = Db.SQL<ImagesSettings>("SELECT s FROM Simplified.Ring6.ImagesSettings s").First;
            if (settings == null)
            {
                settings = new ImagesSettings
                {
                    MaximumFileSize = IllustrationHelper.GetMaximumFileSize(),
                    UploadFolderPath = IllustrationHelper.GetUploadDirectory()
                };
            }
            Data = settings;
        }

        void Handle(Input.Save action) {
            Transaction.Commit();
            UploadHandlers.ReloadHelperPath();
        }

        void Handle(Input.CleanUpFiles action)
        {
            IllustrationHelper.DeleteOldFiles();
            Transaction.Commit();
        }
    }
}
