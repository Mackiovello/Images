using Starcounter;
using Simplified.Ring1;

namespace Images
{
    partial class EditableContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper helper = new IllustrationHelper();

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
            this.URL = string.Empty;
        }
    }
}
