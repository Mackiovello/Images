using System;
using System.Linq;

namespace Images
{
    internal static class RoutingExtensions
    {
        public static IPageRouting GetPageRouting(this RoutingAttribute attribute)
        {
            if (attribute.PageRoutingType == null) throw new ArgumentException($"{nameof(RoutingAttribute.PageRoutingType)} is undefined");
            if (attribute.PageRoutingType.GetInterfaces().All(x => x != typeof(IPageRouting)))
                throw new ArgumentException($"{attribute.PageRoutingType.FullName} is not implemented {nameof(IPageRouting)}");

            return Activator.CreateInstance(attribute.PageRoutingType) as IPageRouting;
        }
    }
}