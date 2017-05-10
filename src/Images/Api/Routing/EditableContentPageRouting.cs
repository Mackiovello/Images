using System.Collections.Generic;
using Simplified.Ring1;
using Starcounter;
using Starcounter.Authorization.Routing;

namespace Images
{
    public class EditableContentPageRouting : IPageRouting
    {
        public List<RoutingPreset> GetRoutingPresets()
        {
            return new List<RoutingPreset>
            {
                new RoutingPreset("/images/partials/illustrations-edit/{?}", GetIllustrationsEditResponse)
            };
        }

        private Response GetIllustrationsEditResponse(RoutingInfo info)
        {
            var illustrationId = info.Arguments[0];
            var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
            if (illustration == null)
            {
                var errorPage = new ErrorPage
                {
                    ErrorText = "Images cannot present an empty illustration"
                };
                return errorPage;
            }

            if (illustration.Content == null)
            {
                illustration.Content = new Content { Name = "Standalone image" };
            }

            return Self.GET("/images/partials/contents-edit/" + illustration.Content.Key);
        }
    }
}