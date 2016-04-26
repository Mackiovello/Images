using Simplified.Ring1;

namespace Images.Helpers
{
    public class ImageValidator
    {
        public static string IsValid(Illustration illustration)
        {
            return !string.IsNullOrEmpty(illustration.Content?.URL) ? string.Empty : "Image cannot be empty!";
        }
    }
}
