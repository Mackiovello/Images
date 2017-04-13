using System;
using Images.Permissions;
using Starcounter;
using Starcounter.Authorization.Authentication;
using Starcounter.Authorization.Core;
using Starcounter.Authorization.Core.Rules;
using Starcounter.Authorization.PageSecurity;
using Starcounter.Authorization.Routing;
using Starcounter.Authorization.Routing.Middleware;

namespace Images
{
    public class AuthorizedHandlers
    {
        public void Register()
        {
            var rules = new AuthorizationRules();
            var enforcement = new AuthorizationEnforcement(rules, new SystemUserAuthentication());
            var router = Router.CreateDefault();

            router.AddMiddleware(new SecurityMiddleware(enforcement,
                info =>
                {
                    var response = Response.FromStatusCode(403);
                    response.Resource = new UnauthorizedPage();
                    return response;
                },
                PageSecurity.CreateThrowingDeniedHandler<Exception>()));

            RegisterHandlers(router);
            RegisterPermissions(rules);
        }

        protected void RegisterHandlers(Router router)
        {
            router.HandleGet<ImagesPage>("/images/partials/images");
        }

        protected void RegisterPermissions(AuthorizationRules rules)
        {
            rules.AddRule(new ClaimRule<ListImages, SystemUserClaim>((claim, permission) => true));
        }
    }
}