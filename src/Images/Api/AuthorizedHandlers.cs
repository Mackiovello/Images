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
            var rules = RegisterRules();

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

            router.RegisterAllFromCurrentAssembly();
        }

        protected AuthorizationRules RegisterRules()
        {
            var rules = new AuthorizationRules();
            rules.AddRule(new ClaimRule<ListImages, SystemUserClaim>((claim, permission) => true));
            return rules;
        }
    }
}