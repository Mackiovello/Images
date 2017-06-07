using System.Collections.Generic;
using Simplified.Ring1;
using Starcounter;
using Starcounter.Authorization.Attributes;
using Starcounter.Authorization.Routing;

namespace Images
{
    [PartialUrl("/images/partials/image/{?}")]
    [RequirePermission(typeof(OpenBasicPages))]
    partial class ImagePage : Json, IBound<Content>, IPageContext<Content>
    {
        protected IllustrationHelper Helper = new IllustrationHelper();
        protected List<string> OldUrls = new List<string>();

        [UriToContext]
        public static Content CreateContext(string[] args)
        {
            var objectId = args.Length > 0 ? args[0] : null;
            if (string.IsNullOrEmpty(objectId))
            {
                var name = "Standalone image";
                var illustration = new Illustration
                {
                    Concept = new Something { Name = name },
                    Content = new Content { Name = name },
                    Name = name
                };
                return illustration.Content;
            }

            return DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Content;
        }

        protected override void OnData()
        {
            base.OnData();

            MaxFileSize = Helper.GetMaximumFileSizeBytes();

            foreach (var s in UploadHandlers.AllowedMimeTypes)
            {
                AllowedMimeTypes.Add().StringValue = s;
            }

            SessionId = Session.Current?.SessionId;
        }

        public void HandleContext(Content context)
        {
            Data = context;
            EditableContent = Self.GET($"/images/partials/contents-edit/{context.Key}");
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