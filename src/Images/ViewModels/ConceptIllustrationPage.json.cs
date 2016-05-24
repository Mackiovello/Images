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

            var content = new Content();
            _currentIllustration.Content = content;
            Data = content;
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
                Data.Name = value;
                OldImageUrl = Data.URL;
            }
        }

        void Handle(Input.Delete action)
        {
            if (Data == null) return;
            Helper.DeleteFile(Data.URL);
            Data?.Delete();
            AddNew(_currentIllustration);
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
