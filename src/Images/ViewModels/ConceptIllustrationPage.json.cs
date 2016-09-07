using System.IO;
using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ConceptIllustrationPage : Page, IBound<Content>
    {
        protected string OldImageUrl;
        protected Transaction ObjectTransaction;
        protected IllustrationHelper Helper = new IllustrationHelper();
        private Illustration _currentIllustration;
        protected IllustrationHelper helper = new IllustrationHelper();

        protected override void OnData()
        {
            base.OnData();

            MaxFileSize = Helper.GetMaximumFileSize();

            foreach (string s in UploadHandlers.AllowedMimeTypes) {
                AllowedMimeTypes.Add().StringValue = s;
            }

            this.SessionId = Session.Current.SessionId;
        }

        public void AddNew(Illustration illustration)
        {
            _currentIllustration = illustration;

            if (illustration.Content == null)
            {
                _currentIllustration.Content = new Content
                {
                    Path = helper.GetUploadDirectoryWithRoot().Replace("/", "\\")
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
            }
        }

        void Handle(Input.Delete action)
        {
            if (Data == null) return;
            Helper.DeleteFile(Data.URL);
            //Data?.Delete();
            //AddNew(_currentIllustration);
            Data.URL = string.Empty;
        }

        void Handle(Input.Save action)
        {
            //Trick to commit just current content
            var contentKey = string.Empty;
            Db.Transact(content =>
            {
                var result = new Content
                {
                    URL = content.Url,
                    Name = content.Name
                };
                contentKey = result.GetObjectID();
            }, new { Url = Data.URL, Name = Data.Name});
            Data =  Db.SQL<Content>("SELECT c FROM Simplified.Ring1.Content c WHERE c.Key = ?", contentKey).First;
            _currentIllustration.Content = Data;
        }
    }
}
