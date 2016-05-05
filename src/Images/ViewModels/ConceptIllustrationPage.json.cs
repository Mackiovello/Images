using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class ConceptIllustrationPage : Page, IBound<Content>
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

        public void RefreshData(string contentId)
        {
            var content = (Content)DbHelper.FromID(DbHelper.Base64DecodeObjectID(contentId));
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
            Data.Delete();
        }

        void Handle(Input.Save action)
        {
            Transaction.Commit();
        }
    }
}
