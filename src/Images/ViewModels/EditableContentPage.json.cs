using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;

namespace Images
{
    partial class EditableContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper helper = new IllustrationHelper();
        protected List<string> oldUrls = new List<string>();

        protected override void OnData()
        {
            base.OnData();

            MaxFileSize = helper.GetMaximumFileSizeBytes();

            foreach (var s in UploadHandlers.AllowedMimeTypes)
            {
                AllowedMimeTypes.Add().StringValue = s;
            }

            ContentPage.Data = Data;
            SessionId = Session.Current.SessionId;
        }

        void Handle(Input.Clear value)
        {
            if (!string.IsNullOrEmpty(this.URL))
            {
                oldUrls.Add(this.URL);
            }
            this.URL = string.Empty;
        }

        void Handle(Input.URL value)
        {
            if (!string.IsNullOrEmpty(value.OldValue))
            {
                oldUrls.Add(value.OldValue);
            }
        }

        void Handle(Input.Save value)
        {
            SaveChanges();
        }

        public void SaveChanges()
        {
            foreach (var url in oldUrls)
            {
                helper.DeleteFile(url);
            }
            Transaction.Commit();
            RedirectUrl = "/images";
        }
    }
}
