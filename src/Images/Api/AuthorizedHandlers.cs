using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Dictionary<Type, PartialUrlAttribute> partialPages = Assembly.GetAssembly(typeof(AuthorizedHandlers))
                .GetTypes()
                .Select(type => Tuple.Create(type, type.GetCustomAttribute<PartialUrlAttribute>()))
                .Where(tuple => tuple.Item2 != null)
                .ToDictionary(x => x.Item1, x => x.Item2);

            Router outsideFacingRouter = CreateApiRouter();
            Router selfOnlyRouter = CreatePartialsRouter();

            foreach (var tuple in partialPages)
            {
                selfOnlyRouter.HandleGet(tuple.Value.UriPartialVersion, tuple.Key, new HandlerOptions { SelfOnly = true });
                outsideFacingRouter.HandleGet(tuple.Value.UriApiVersion, tuple.Key);
            }
        }

        protected Router CreatePartialsRouter()
        {
            var router = Router.CreateDefault();
            router.AddMiddleware(new ContextMiddleware());
            router.AddMiddleware(new DbScopeMiddleware(defaultValue: true));
            return router;
        }

        protected Router CreateApiRouter()
        {
            var router = new Router(info =>
            {
                var partialUri = GetPartialUri(info.Request.Uri);
                return Self.GET(partialUri);
            });

            router.AddMiddleware(new MasterPageMiddleware());
            AddSecurityMiddleware(router);

            return router;
        }

        protected void AddSecurityMiddleware(Router router)
        {
            var rules = GetAuthorizationRules(router);
            router.AddMiddleware(new SecurityMiddleware(
                new AuthorizationEnforcement(rules, new SystemUserAuthentication()),
                info => SessionHelper.GetMaster(() => new UnauthorizedPage()),
                PageSecurity.CreateThrowingDeniedHandler<Exception>()));
        }

        private AuthorizationRules GetAuthorizationRules(Router router)
        {
            var rules = new AuthorizationRules();
            rules.AddRule(new ClaimRule<OpenBasicPages, SystemUserClaim>((claim, permission) => true));
            return rules;
        }

        private string GetPartialUri(string apiUri)
        {
            return apiUri.Insert(AppHelper.AppName.Length + 1, AppHelper.PartialUriPart);
        }
    }
}