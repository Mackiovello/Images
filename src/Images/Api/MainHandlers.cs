using Starcounter;
using Simplified.Ring1;
using Simplified.Ring2;
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

            RegisterLauncherHooks();
            RegisterMapperHandlers();
        }

        protected void RegisterLauncherHooks()
        {
            Handle.GET("/images/app-name", () => new AppName());
            Handle.GET("/images/menu", () => new Page { Html = "/Images/viewmodels/AppMenu.html" });
        }

        protected void RegisterMapperHandlers()
        {
            Blender.MapUri("/images/menu", "menu");
            Blender.MapUri("/images/app-name", "app-name");
            Blender.MapUri("/images/partials/settings", "settings");

            #region OntologyMap
            Blender.MapUri<Person>("/images/partials/somethings-edit/{?}");
            Blender.MapUri<Organization>("/images/partials/somethings-edit/{?}");
            Blender.MapUri<Product>("/images/partials/somethings-edit/{?}");

            Blender.MapUri<ChatMessage>("/images/partials/somethings-single/{?}",
                paramsFrom => paramsFrom,
                paramsTo =>
                {
                    var objectId = paramsTo[0];
                    var message = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as ChatMessage;
                    return message.IsDraft ? null : paramsTo;
                });
            Blender.MapUri<ChatMessage>("/images/partials/images-draft/{?}",
                paramsFrom => paramsFrom,
                paramsTo =>
                {
                    var objectId = paramsTo[0];
                    var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                    return chatMessage.IsDraft ? paramsTo : null;
                });
            Blender.MapUri<ChatAttachment>("/images/partials/illustrations-edit/{?}",
                paramsFrom => paramsFrom,
                paramsTo =>
                {
                    var objectId = paramsTo[0];
                    var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Illustration;
                    if (illustration == null)
                    {
                        return null;
                    }
                    return paramsTo;
                });

            #region For Chatter
            Blender.MapUri<EditAnnouncement>("/images/partials/illustrations-edit/{?}");
            Blender.MapUri<PreviewAnnouncement>("/images/partials/illustrations/{?}");
            Blender.MapUri<ChatDraftAnnouncement>("/images/partials/imagedraftannouncement/{?}");
            Blender.MapUri<ChatWarning>("/images/partials/imagewarning/{?}");
            #endregion

            #endregion
        }
    }
}
