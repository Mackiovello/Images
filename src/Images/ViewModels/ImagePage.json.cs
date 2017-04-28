using Starcounter;
using Simplified.Ring1;
using System.Collections.Generic;
using Starcounter.Authorization.Routing;

namespace Images
{
    [PartialUrl("/images/partials/image/{?}")]
    partial class ImagePage : Json, IBound<Content>, IPageContext<Content>
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