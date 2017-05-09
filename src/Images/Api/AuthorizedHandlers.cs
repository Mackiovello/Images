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
                        routerKit.Router = CreateSecondaryUriRouter(routerKit.PageCreators);
                        routerKits.Add(routerKit);
                    }

                    routerKit.PageCreators.Add(routing.Key, preset.PageCreator);
                    routerKit.Router.HandleGet(preset.Uri, routing.Key);
                }
            }
        }

        private Router CreateSecondaryUriRouter(Dictionary<Type, Func<RoutingInfo, Response>> pageCreators)
        {
            var router = new Router(info => pageCreators[info.SelectedPageType](info));

            AddSecurityMiddleware(router, info =>
            {
                var response = Response.FromStatusCode(403);
                response.Resource = new Json();
                return response;
            });

            return router;
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
                var partialUri = PartialUrlAttribute.GetPartialUri(info.Request.Uri, "images");
                return Self.GET(partialUri);
            });

            router.AddMiddleware(new MasterPageMiddleware());
            AddSecurityMiddleware(router, info =>
            {
                var response = Response.FromStatusCode(403);
                response.Resource = new UnauthorizedPage();
                return response;
            });

            return router;
        }

        private void AddSecurityMiddleware(Router router, Func<RoutingInfo, Response> unauthorizedHandler)
        {
            var rules = GetAuthorizationRules(router);
            router.AddMiddleware(new SecurityMiddleware(
                new AuthorizationEnforcement(rules, new SystemUserAuthentication()),
                unauthorizedHandler,
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
            public Router Router { get; set; }
            public Dictionary<Type, Func<RoutingInfo, Response>> PageCreators { get; }

            public RouterPageCreatorsKit()
            {
                PageCreators = new Dictionary<Type, Func<RoutingInfo, Response>>();
            }
        }
    }
}