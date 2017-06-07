using System;
using Starcounter;
using Starcounter.Authorization.Routing;

namespace Images
{
    public class MasterPageMiddleware : IPageMiddleware
    {
        public Response Run(RoutingInfo routingInfo, Func<Response> next)
        {
            MasterPage master = SessionHelper.GetMaster();
            master.CurrentPage = next();
            return master;
        }
    }
}