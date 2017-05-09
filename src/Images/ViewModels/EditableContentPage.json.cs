using System.Collections.Generic;
using Simplified.Ring1;
using Starcounter;
using Starcounter.Authorization.Attributes;
using Starcounter.Authorization.Routing;

namespace Images
{
    [PartialUrl("/images/partials/contents-edit/{?}")]
    [Routing(typeof(EditableContentPageRouting))]
    [RequirePermission(typeof(OpenBasicPages))]
    partial class EditableContentPage : Json, IBound<Content>
    {
        protected IllustrationHelper Helper = new IllustrationHelper();
        protected List<string> OldUrls = new List<string>();
        public string SessionId => Session.Current?.SessionId;

        [UriToContext]
        public static Content CreateContext(string[] args)
        {
            var objectId = args[0];
            var content = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Content;

            if (content == null)
            {
                var name = "Standalone image";
                var illustration = new Illustration
                {
                    Concept = new Something { Name = name },
                    Content = new Content { Name = name },
                    Name = name
                };
                content = illustration.Content;
            }
            return content;
        }

        protected override void OnData()
        {
            base.OnData();

            MaxFileSize = Helper.GetMaximumFileSizeBytes();

            foreach (var mimeType in UploadHandlers.AllowedMimeTypes)
            {
                AllowedMimeTypes.Add().StringValue = mimeType;
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

            if (string.IsNullOrEmpty(value.Value))
            {
                Path = Helper.GetUploadDirectoryWithRoot().Replace("/", "\\");
            }
        }
    }
}