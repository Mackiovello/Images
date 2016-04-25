using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ConceptPageIllustration : Page, IBound<Illustration>
    {
        protected string OldImageUrl;
        protected IllustrationHelper Helper = new IllustrationHelper();

        protected override void OnData()
        {
            base.OnData();

            MaxFileSize = UploadHandlers.MaxFileSize;

            foreach (string s in UploadHandlers.AllowedMimeTypes) {
                AllowedMimeTypes.Add().StringValue = s;
            }
        }

        public void RefreshData(string illustrationId)
        {
            var illustration = (Illustration)DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId));
            Data = illustration;
        }

        public string URL
        {
            get {
                return Data?.Content?.URL;
            }
            set {
                if (Data.Content == null)
                {
                    Data.Content = new Content();
                }
                Data.Content.URL = value;
                Data.Name = value;
                OldImageUrl = Data.Content.URL;
            }
        }

        void Handle(Input.Delete action)
        {
            if (Data == null) return;
            Helper.DeleteFile(Data);

            Data.Content?.Delete();
            Data.Delete();
        }

        void Handle(Input.Save action)
        {
            Transaction.Commit();
        }
    }
}
