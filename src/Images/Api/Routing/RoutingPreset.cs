using System;
using Starcounter;
using Starcounter.Authorization.Routing;

namespace Images
{
    public class RoutingPreset
    {
        public string Uri { get; set; }
        public Func<RoutingInfo, Response> PageCreator { get; set; }
    }
}