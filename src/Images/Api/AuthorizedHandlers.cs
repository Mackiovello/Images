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
            Type[] assemblyTypes = Assembly.GetAssembly(typeof(AuthorizedHandlers)).GetTypes();

            Dictionary<Type, PartialUrlAttribute> partialPages = assemblyTypes
                .Select(type => Tuple.Create(type, type.GetCustomAttribute<PartialUrlAttribute>()))
                .Where(tuple => tuple.Item2 != null)
                .ToDictionary(x => x.Item1, x => x.Item2);

            RegisterPrimaryRouting(partialPages);

            Dictionary<Type, IPageRouting> secondaryRoutings = assemblyTypes
                .Select(type => Tuple.Create(type, type.GetCustomAttribute<RoutingAttribute>()))
                .Where(tuple => tuple.Item2 != null)
                .ToDictionary(x => x.Item1, x => x.Item2.GetPageRouting());

            RegisterSecondaryRouting(secondaryRoutings);
        }

        protected void RegisterPrimaryRouting(Dictionary<Type, PartialUrlAttribute> partialPages)
        {
            Router outsideFacingRouter = CreateApiRouter();
            Router selfOnlyRouter = CreatePartialsRouter();

            foreach (var tuple in partialPages)
            {
                selfOnlyRouter.HandleGet(tuple.Value.UriPartialVersion, tuple.Key, new HandlerOptions { SelfOnly = true });
                outsideFacingRouter.HandleGet(tuple.Value.UriApiVersion, tuple.Key);
            }
        }

        protected void RegisterSecondaryRouting(Dictionary<Type, IPageRouting> pageRoutings)
        {
            var routerKits = new List<RouterPageCreatorsKit>();
            foreach (var routing in pageRoutings)
            {
                var routingPresets = routing.Value.GetRoutingPresets();

                foreach (var preset in routingPresets)
                {
                    var routerKit = routerKits.FirstOrDefault(x => !x.PageCreators.ContainsKey(routing.Key));

                    if (routerKit == null)
                    {
                        routerKit = new RouterPageCreatorsKit();
                        routerKits.Add(routerKit);

                        routerKit.PartialsRouter = CreateSecondaryPartialsRouter(routerKit.PageCreators);
                        routerKit.ApiRouter = CreateApiRouter();
                    }

                    routerKit.PageCreators.Add(routing.Key, preset.PageCreator);

                    routerKit.PartialsRouter.HandleGet(preset.PartialUri, routing.Key, new HandlerOptions {SelfOnly = true});
                    routerKit.ApiRouter.HandleGet(preset.ApiUri, routing.Key);
                }
            }
        }

        private Router CreateSecondaryPartialsRouter(Dictionary<Type, Func<RoutingInfo, Response>> pageCreators)
        {
            return new Router(info => pageCreators[info.SelectedPageType](info));
        }

        private Router CreatePartialsRouter()
        {
            var router = Router.CreateDefault();
            router.AddMiddleware(new ContextMiddleware());
            router.AddMiddleware(new DbScopeMiddleware(defaultValue: true));
            return router;
        }

        private Router CreateApiRouter()
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

        private void AddSecurityMiddleware(Router router)
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


        protected class RouterPageCreatorsKit
        {
            public Router ApiRouter { get; set; }
            public Router PartialsRouter { get; set; }
            public Dictionary<Type, Func<RoutingInfo, Response>> PageCreators { get; }

            public RouterPageCreatorsKit()
            {
                PageCreators = new Dictionary<Type, Func<RoutingInfo, Response>>();
            }
        }

        private string GetPartialUri(string apiUri)
        {
            return apiUri.Insert(AppHelper.AppName.Length + 1, AppHelper.PartialUriPart);
        }
    }
}