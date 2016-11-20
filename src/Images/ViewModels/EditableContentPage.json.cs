using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;
using System;

namespace Images
{
    partial class EditableContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper helper = new IllustrationHelper();
        protected List<string> oldUrls = new List<string>();

        protected override void OnData()
        {
            base.OnData();

            this.MaxFileSize = helper.GetMaximumFileSize();

            foreach (string s in UploadHandlers.AllowedMimeTypes)
            {
                this.AllowedMimeTypes.Add().StringValue = s;
            }

            this.ContentPage.Data = Data;

            this.SessionId = Session.Current.SessionId;
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
            for (int i = 0; i < oldUrls.Count; i++)
            {
                helper.DeleteFile(oldUrls[i]);
            }
            this.Transaction.Commit();
        }
    }
}
