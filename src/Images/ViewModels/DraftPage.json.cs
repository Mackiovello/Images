using Simplified.Ring1;
using Simplified.Ring6;
using Starcounter;
using Starcounter.Authorization.Routing;
using Starcounter.Authorization.Routing.Middleware;

namespace Images
{
    [PartialUrl("/images/partials/images-draft/{?}")]
    [UseDbScope(false)]
    partial class DraftPage : Json, IPageContext<Illustration>
    {
        [UriToContext]
        public static Illustration CreateContext(string[] args)
        {
            var chatMessageId = args[0];
            var chatMessage = (ChatMessage) DbHelper.FromID(DbHelper.Base64DecodeObjectID(chatMessageId));
            var relation = new Illustration
            {
                Concept = chatMessage,
                Content = new Content { Name = "Content for illustration of a chat message" },
            };
            return relation;
        }

        public void HandleContext(Illustration relation)
        {
            SubPage = Self.GET("/images/partials/imagedraftannouncement/" + relation.GetObjectID());
        }
    }
}
