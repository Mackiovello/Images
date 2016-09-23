using Starcounter;
using Simplified.Ring6;

namespace Images
{
    partial class SettingsPage : Page, IBound<ImagesSettings>
    {
        private IllustrationHelper helper = new IllustrationHelper();

        public void LoadDefaultData()
        {
            var settings = Db.SQL<ImagesSettings>("SELECT s FROM Simplified.Ring6.ImagesSettings s").First;
            if (settings == null)
            {
                settings = new ImagesSettings
                {
                    MaximumFileSize = helper.GetMaximumFileSizeBytes(),
                    UploadFolderPath = helper.GetUploadDirectory()
                };
            }
            Data = settings;
        }

        public decimal MaximumFileSizeMiB
        {
            get { return helper.GetMaximumFileSizeMiB(); }
            set { helper.SetMaximumFileSizeMiB(value); }
        }


        void Handle(Input.Save action)
        {
            Transaction.Commit();
            UploadHandlers.ReloadHelperPath();
        }
    }
}
