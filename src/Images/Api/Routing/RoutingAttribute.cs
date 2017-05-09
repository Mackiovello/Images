using System;

namespace Images
{
    /// <summary>
    /// Specifies a routing info
    /// </summary>
    public class RoutingAttribute : Attribute
    {
        /// <summary>
        /// A type which implements <see cref="IPageRouting"/>
        /// </summary>
        public Type PageRoutingType { get; }

        /// <param name="pageRoutingType">A type which implements <see cref="IPageRouting"/></param>
        public RoutingAttribute(Type pageRoutingType)
        {
            PageRoutingType = pageRoutingType;
        }
    }
}