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
                    MaximumFileSize = IllustrationHelper.DefaultMaximumFileSize,
                    UploadFolderPath = helper.GetUploadDirectory()
                };
            }
            Data = settings;
        }

        public decimal MaximumFileSizeMiB
        {
            get { return helper.BytesToMiB(Data.MaximumFileSize); }
            set { Data.MaximumFileSize = helper.MiBToBytes(value); }
        }


        void Handle(Input.Save action)
        {
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
