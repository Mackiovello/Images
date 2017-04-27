using System;
using Starcounter;
using Starcounter.Authorization.Routing;

namespace Images
{
    public class MasterPageMiddleware : IPageMiddleware
    {
        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            var session = Session.Current ?? new Session(SessionOptions.PatchVersioning);

            var master = session?.Data as MasterPage ?? new MasterPage();
            master.Session = session;
            master.CurrentPage = next();
            return master;
        }
    }
}