using Simplified.Ring1;
using Starcounter;
using Starcounter.Authorization.Attributes;
using Starcounter.Authorization.Routing;
using Starcounter.Authorization.Routing.Middleware;

namespace Images
{
    [PartialUrl("/images/partials/somethings-single-static/{?}")]
    [UseDbScope(false)]
    [RequirePermission(typeof(OpenBasicPages))]
    partial class IllustrationSimplePage : Json, IBound<Illustration>
    {
        [UriToContext]
        public static Illustration CreateContext(string[] args)
        {
            var objectId = args[0];
            var something = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Something;
            return something?.Illustration;
        }
    }
}
