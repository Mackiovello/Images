using System;

namespace Images
{
    /// <summary>
    /// Page marked with this attribute will be registered under provided URI <see cref="UriPartialVersion"/>
    /// with any permission checking disabled, but with Self.Only option.
    /// It will also be registered "publicly" with permission checking enabled under URI with "/partials"
    /// </summary>
    public sealed class PartialUrlAttribute: Attribute
    {
        public string UriPartialVersion { get; }
        public string UriApiVersion { get; }

        public PartialUrlAttribute(string uriPartialVersion)
        {
            UriPartialVersion = uriPartialVersion;
            UriApiVersion = uriPartialVersion.Replace("/partials", "");
        }

        public static string GetPartialUri(string apiUri, string appName)
        {
            var partialUri = apiUri.Insert(appName.Length + 1, "/partials");
            return partialUri;
        }
    }
}