using Simplified.Ring1;

namespace Images.Helpers
{
    public class ImageValidator
    {
        public static string IsValid(Content content)
        {
            return !string.IsNullOrEmpty(content?.URL) ? string.Empty : "Image cannot be empty!";
        }
    }
}
