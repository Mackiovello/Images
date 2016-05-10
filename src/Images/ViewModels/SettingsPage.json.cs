using Starcounter;
using Simplified.Ring6;

namespace Images {
    partial class SettingsPage : Page, IBound<ImagesSettings> {
        public void LoadDefaultData()
        {
            var settings = Db.SQL<ImagesSettings>("SELECT s FROM Simplified.Ring6.ImagesSettings s").First;
            if (settings == null)
            {
                var helper = new IllustrationHelper();
                settings = new ImagesSettings
                {
                    MaximumFileSize = helper.GetMaximumFileSize(),
                    UploadFolderPath = helper.GetUploadDirectory()
                };
            }
            Data = settings;
        }

        void Handle(Input.Save action) {
            Transaction.Commit();
            new UploadHandlers().RegisterSharedFolder();
        }
    }
}
