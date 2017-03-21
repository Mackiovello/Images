using Starcounter;
using Simplified.Ring6;

namespace Images
{
    partial class SettingsPage : Json, IBound<ImagesSettings>
    {
        private readonly IllustrationHelper _illustrationHelper = new IllustrationHelper();
        public void LoadDefaultData()
        {
            Data = Db.SQL<ImagesSettings>("SELECT s FROM Simplified.Ring6.ImagesSettings s").First;
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
