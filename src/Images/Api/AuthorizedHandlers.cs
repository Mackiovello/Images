using System;
using System.Linq;
using System.Reflection;
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
            Router outsideFacingRouter = CreateOutsideFacingRouter();
            Router selfOnlyRouter = CreateSelfOnlyRouter();

            var partialPages = Assembly.GetAssembly(typeof(AuthorizedHandlers))
                .GetTypes()
                .Select(type => Tuple.Create(type, type.GetCustomAttribute<PartialUrlAttribute>()))
                .Where(tuple => tuple.Item2 != null);

            foreach (var tuple in partialPages)
            {
                selfOnlyRouter.HandleGet(tuple.Item2.UriPartialVersion, tuple.Item1, new HandlerOptions() {SelfOnly = true});
                outsideFacingRouter.HandleGet(tuple.Item2.UriPartialVersion.Replace("/partials", ""), tuple.Item1);
            }
        }

        private static Router CreateSelfOnlyRouter()
        {
            var router = Router.CreateDefault();
            router.AddMiddleware(new ContextMiddleware());
            router.AddMiddleware(new DbScopeMiddleware(defaultValue: true));
            return router;
        }

        private Router CreateOutsideFacingRouter()
        {
            var router = CreateSelfOnlyRouter();
            router.AddMiddleware(new MasterPageMiddleware());
            var rules = RegisterRules();

            var enforcement = new AuthorizationEnforcement(rules, new SystemUserAuthentication());
            router.AddMiddleware(new SecurityMiddleware(enforcement,
                info => 403,
                PageSecurity.CreateThrowingDeniedHandler<Exception>()));

            return router;
        }

        private AuthorizationRules RegisterRules()
        {
            var rules = new AuthorizationRules();
            rules.AddRule(new ClaimRule<ListImages, SystemUserClaim>((claim, permission) => true));
            return rules;
        }
    }
}