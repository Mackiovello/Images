using Simplified.Ring1;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Authorization.Routing;
using Starcounter.Authorization.Routing.Middleware;

namespace Images
{
    [PartialUrl("/images/partials/imagewarning/{?}")]
    [UseDbScope(false)]
    partial class ConceptIllustrationWarningPage : Json, IPageContext<Illustration>
    {
        public void HandleContext(Illustration illustration)
        {
            Warning = ImageValidator.IsValid(illustration.Content);
            var relation = Db.SQL<ChatWarning>(@"Select m from Simplified.Ring6.ChatWarning m Where m.ErrorRelation = ?", illustration).First;
            if (!string.IsNullOrEmpty(Warning))
            {
                if (relation == null)
                {
                    new ChatWarning
                    {
                        ErrorRelation = illustration
                    };
                }
            }
            else
            {
                relation?.Delete();
            }
        }
    }
}
