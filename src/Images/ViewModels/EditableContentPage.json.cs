using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;

namespace Images
{
    partial class EditableContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper Helper = new IllustrationHelper();
        protected List<string> OldUrls = new List<string>();
        public string SessionId => Session.Current?.SessionId;

        protected override void OnData()
        {
            base.OnData();
            
            MaxFileSize = Helper.GetMaximumFileSizeBytes();

            foreach (var s in UploadHandlers.AllowedMimeTypes)
            {
                AllowedMimeTypes.Add().StringValue = s;
            }

            ContentPage.Data = Data;
        }

        void Handle(Input.Clear value)
        {
            if (!string.IsNullOrEmpty(this.URL))
            {
                OldUrls.Add(URL);
            }
            URL = string.Empty;
            Path = string.Empty;
        }

        void Handle(Input.URL value)
        {
            if (!string.IsNullOrEmpty(value.OldValue))
            {
                OldUrls.Add(value.OldValue);
            }

            Path = Helper.GetUploadDirectoryWithRoot().Replace("/","\\");            
        }
    }
}