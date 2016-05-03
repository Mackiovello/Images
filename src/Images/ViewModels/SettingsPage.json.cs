using Starcounter;
using Simplified.Ring6;

namespace Images {
    partial class SettingsPage : Page, IBound<ImageSettings> {
        public void LoadDefaultData()
        {
            var settings = Db.SQL<ImageSettings>("SELECT s FROM Simplified.Ring6.ImageSettings s").First;
            if (settings == null)
            {
                var helper = new IllustrationHelper();
                settings = new ImageSettings
                {
                    MaximumFileSize = helper.GetMaximumFileSize(),
                    UploadFolderPath = helper.GetUploadDirectory()
                };
            }
            Data = settings;
        }

        void Handle(Input.Save action) {
            Transaction.Commit();
        }
    }
}
