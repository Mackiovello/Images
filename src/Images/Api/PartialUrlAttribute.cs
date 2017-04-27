using System;

namespace Images
{
    /// <summary>
    /// Page marked with this attribute will be registered under provided URI
    ///  with any permission checking disabled, but with Self.Only option.
    /// It will also be registered "publicly" with permission checking enabled under URI with "/partials" removed
    /// </summary>
    public sealed class PartialUrlAttribute: Attribute
    {
        public string UriPartialVersion { get; }

        public PartialUrlAttribute(string uriPartialVersion)
        {
            UriPartialVersion = uriPartialVersion;
        }
    }
}