using System.Collections.Generic;
using Simplified.Ring1;
using Simplified.Ring6;
using Starcounter;

namespace Images
{
    public class DraftPageRouting : IPageRouting
    {
        public List<RoutingPreset> GetRoutingPresets()
        {
            return new List<RoutingPreset>
            {
                new RoutingPreset("/images/partials/images-draft/{?}",
                    info =>
                    {
                        var chatMessageId = info.Arguments[0];
                        var chatMessage = (ChatMessage) DbHelper.FromID(DbHelper.Base64DecodeObjectID(chatMessageId));
                        var relation = new Illustration
                        {
                            Concept = chatMessage,
                            Content = new Content {Name = "Content for illustration of a chat message"},
                        };
                        var draft = new DraftPage
                        {
                            SubPage = Self.GET("/images/partials/imagedraftannouncement/" + relation.GetObjectID())
                        };
                        return draft;
                    }),
                new RoutingPreset("/images/partials/imagedraftannouncement/{?}", info => new Page())
            };
        }
    }
}