using System.Collections.Generic;
using Simplified.Ring1;
using Starcounter;
using Starcounter.Authorization.Routing;

namespace Images
{
    public class ContentPageRouting : IPageRouting
    {
        public List<RoutingPreset> GetRoutingPresets()
        {
            return new List<RoutingPreset>
            {
                new RoutingPreset
                {
                    Uri = "/images/partials/illustrations/{?}",
                    PageCreator = GetIllustrationsResponse
                },
                new RoutingPreset
                {
                    Uri = "/images/partials/somethings-single/{?}",
                    PageCreator = GetSomethingsSinglePage
                }
            };
        }

        private Response GetSomethingsSinglePage(RoutingInfo info)
        {
            var objectId = info.Arguments[0];
            var something = (Something) DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
            var illustration = Db.SQL<Illustration>(@"Select m from Simplified.Ring1.Illustration m Where m.ToWhat = ?", something).First;
            return illustration == null
                ? new Page()
                : Self.GET("/images/partials/illustrations/" + illustration.GetObjectID());
        }

        private Response GetIllustrationsResponse(RoutingInfo info)
        {
            var illustrationId = info.Arguments[0];
            var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
            if (illustration?.Content == null)
            {
                var errorPage = new ErrorPage
                {
                    ErrorText = "Images cannot present an illustration without content"
                };
                return errorPage;
            }
            return Self.GET("/images/partials/contents/" + illustration.Content.Key);
        }
    }
}