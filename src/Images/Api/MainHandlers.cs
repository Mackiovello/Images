using Starcounter;
using Simplified.Ring1;
using Simplified.Ring3;
using Simplified.Ring6;
using Simplified.Ring6.Images;

namespace Images
{
    internal class MainHandlers
    {
        public void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/images", () => Self.GET("/images/images"));
            Handle.GET("/images/image", () => Self.GET("/images/image/"));

            UploadHandlers.GET("/images/images", task =>
            {
                Session.ScheduleTask(task.SessionId, (s, id) =>
                {
                    s.CalculatePatchAndPushOnWebSocket();
                });
            });

            RegisterPartials();
            RegisterLauncherHooks();
            RegisterMapperHandlers();
        }

        protected void RegisterLauncherHooks()
        {
            Handle.GET("/images/app-name", () => new AppName());
            Handle.GET("/images/menu", () => new Page { Html = "/Images/viewmodels/AppMenu.html" });
        }

        protected void RegisterPartials()
        {
            #region Custom application handlers
            Handle.GET("/images/partials/images-draft/{?}", (string chatMessageId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(chatMessageId));
                var relation = new Illustration
                {
                    Concept = chatMessage,
                    Content = new Simplified.Ring1.Content() { Name = "Content for illustration of a chat message" },
                };
                var draft = new DraftPage
                {
                    SubPage = Self.GET("/images/partials/imagedraftannouncement/" + relation.GetObjectID())
                };
                return draft;
            });

            Handle.GET("/images/partials/imagedraftannouncement/{?}", (string objectPath) => new Page());

            Handle.GET("/images/partials/imagewarning/{?}", (string illustrationId) =>
            {
                var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                if (illustration == null) return new Page();

                var page = new ConceptIllustrationWarningPage();
                page.RefreshData(illustration);
                return page;
            });
            #endregion

        }

        protected void RegisterMapperHandlers()
        {
            UriMapping.Map("/images/menu", UriMapping.MappingUriPrefix + "/menu");
            UriMapping.Map("/images/app-name", UriMapping.MappingUriPrefix + "/app-name");
            UriMapping.Map("/images/partials/settings", UriMapping.MappingUriPrefix + "/settings");

            #region Wrapper URI handlers for usage in OntologyMap
            Handle.GET("/images/partials/concept-somebody/{?}", (string objectId) =>
            {
                return Self.GET("/images/partials/somethings-edit/" + objectId);
            });
            Handle.GET("/images/partials/concept-vendible/{?}", (string objectId) =>
            {
                return Self.GET("/images/partials/somethings-edit/" + objectId);
            });
            Handle.GET("/images/partials/concept-chatmessage/{?}", (string objectId) =>
            {
                return Self.GET("/images/partials/somethings-single/" + objectId);
            });
            Handle.GET("/images/partials/concept-chatattachment/{?}", (string objectId) =>
            {
                return Self.GET("/images/partials/illustrations-edit/" + objectId);
            });
            # endregion

            #region OntologyMap
            UriMapping.OntologyMap("/images/partials/concept-somebody/{?}", typeof(Somebody).FullName, null, null);
            UriMapping.OntologyMap("/images/partials/concept-vendible/{?}", typeof(Product).FullName, null, null);
            UriMapping.OntologyMap("/images/partials/concept-chatmessage/{?}", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var message = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as ChatMessage;
                return message.IsDraft ? null : objectId;
            });
            UriMapping.OntologyMap("/images/partials/images-draft/{?}", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                return chatMessage.IsDraft ? objectId : null;
            });
            UriMapping.OntologyMap("/images/partials/concept-chatattachment/{?}", typeof(ChatAttachment).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Illustration;
                if (illustration == null)
                {
                    return null;
                }
                return objectId;
            });

            #region For Chatter
            UriMapping.OntologyMap("/images/partials/illustrations-edit/@w", typeof(EditAnnouncement).FullName);
            UriMapping.OntologyMap("/images/partials/illustrations/@w", typeof(PreviewAnnouncement).FullName);
            UriMapping.OntologyMap("/images/partials/imagedraftannouncement/{?}", typeof(ChatDraftAnnouncement).FullName);
            UriMapping.OntologyMap("/images/partials/imagewarning/{?}", typeof(ChatWarning).FullName);
            #endregion

            #endregion
        }
    }
}
