using System;
using Starcounter;
using Starcounter.Authorization.Routing;

namespace Images
{
    public class RoutingPreset
    {
        public string ApiUri { get; set; }
        public string PartialUri { get; set; }
        public Func<RoutingInfo, Response> PageCreator { get; set; }

        public RoutingPreset(string partialUri, Func<RoutingInfo, Response> pageCreator)
        {
            if (pageCreator == null) throw new ArgumentNullException(nameof(pageCreator));
            if (string.IsNullOrEmpty(partialUri))
                throw new ArgumentException("Value cannot be null or empty.", nameof(partialUri));

            PartialUri = partialUri;
            ApiUri = partialUri.Replace(AppHelper.PartialUriPart, string.Empty);
            PageCreator = pageCreator;
        }
    }
}