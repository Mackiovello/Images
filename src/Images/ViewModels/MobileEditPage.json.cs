using System.IO;
using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class MobileEditPage : Json, IBound<Content>
    {
        protected string OldImageUrl;
        protected Transaction ObjectTransaction;
        protected IllustrationHelper Helper = new IllustrationHelper();
        private Illustration _currentIllustration;

        protected override void OnData()
        {
            base.OnData();

            MaxFileSize = Helper.GetMaximumFileSizeBytes();

            foreach (string s in UploadHandlers.AllowedMimeTypes) {
                AllowedMimeTypes.Add().StringValue = s;
            }

            this.SessionId = Session.Current.SessionId;
        }

        public void Handle(Input.ShowLightBox action)
        {
            IsLightBoxVisible = true;
        }

        public void AddNew(Illustration illustration)
        {
            _currentIllustration = illustration;

            if (illustration.Content == null)
            {
                _currentIllustration.Content = new Content
                {
                    Path = Helper.GetUploadDirectoryWithRoot().Replace("/", "\\")
                };
            }
            Data = _currentIllustration.Content;
        }

        public string URL
        {
            get {
                return Data?.URL;
            }
            set {
                if (Data == null)
                {
                    Data = new Content();
                }
                Data.URL = value;
                Data.Name = Path.GetFileName(value);
                OldImageUrl = Data.URL;
                Transaction.Commit();
            }
        }

        void Handle(Input.Delete action)
        {
            if (Data == null) return;
            Helper.DeleteFile(Data.URL);
            Data.URL = string.Empty;
            Transaction.Commit();
        }
    }
}
