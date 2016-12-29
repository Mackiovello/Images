using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;

namespace Images
{
    partial class ImagePage : Json, IBound<Content>
    {
        protected IllustrationHelper Helper = new IllustrationHelper();
        protected List<string> OldUrls = new List<string>();

        protected override void OnData()
        {
            base.OnData();

            MaxFileSize = Helper.GetMaximumFileSizeBytes();

            foreach (var s in UploadHandlers.AllowedMimeTypes)
            {
                AllowedMimeTypes.Add().StringValue = s;
            }
            
            SessionId = Session.Current.SessionId;
        }

        void Handle(Input.URL value)
        {
            if (!string.IsNullOrEmpty(value.OldValue))
            {
                OldUrls.Add(value.OldValue);
            }
        }

        void Handle(Input.Save value)
        {
            SaveChanges();
        }

        void Handle(Input.Cancel action)
        {
            RedirectUrl = "/images";
        }

        public void SaveChanges()
        {
            foreach (var url in OldUrls)
            {
                Helper.DeleteFile(url);
            }
            Transaction.Commit();
            RedirectUrl = "/images";
        }
    }
}