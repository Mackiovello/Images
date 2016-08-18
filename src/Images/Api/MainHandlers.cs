using Images.Helpers;
﻿using System.Web.UI.WebControls;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring3;
using Simplified.Ring6;
using Simplified.Ring6.Images;
using Page = Starcounter.Page;

namespace Images {
    internal class MainHandlers {
        public void Register() {
            Handle.GET("/images/standalone", () => {
                Session session = Session.Current;

                if (session != null && session.Data != null) {
                    return session.Data;
                }

                StandalonePage standalone = new StandalonePage();

                if (session == null) {
                    session = new Session(SessionOptions.PatchVersioning);
                    standalone.Html = "/Images/viewmodels/StandalonePage.html";
                }

                standalone.Session = session;
                return standalone;
            });

            // Workspace root (Launchpad)
            Handle.GET("/images", (Request request) => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.CurrentPage = Self.GET("/images/partials/images");

                    return master;
                });
            });

            Handle.GET("/images/image/{?}", (string objectId) => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.CurrentPage = Self.GET<IllustrationPage>("/Images/partials/image/" + objectId);
                    return master;
                });
            });

            Handle.GET("/images/concept/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    return Self.GET<ConceptPage>("/images/partials/concept/" + objectId);
                });
            });

            this.RegisterPartials();
            this.RegisterLauncherHooks();
            this.RegisterMapperHandlers();

            IllustrationHelper helper = new IllustrationHelper();

            helper.DeleteOldFiles();
        }

        protected StandalonePage GetMaster() {
            return Self.GET<StandalonePage>("/images/standalone");
        }

        protected void RegisterLauncherHooks() {
            Handle.GET("/images/app-name", () => {
                return new AppName();
            });

            // Menu
            Handle.GET("/images/menu", () => {
                return new Page() { Html = "/Images/viewmodels/AppMenu.html" };
            });
        }

        protected void RegisterPartials() {
            Handle.GET("/images/partials/images", (Request request) => {
                IllustrationsPage page = new IllustrationsPage() {
                    Html = "/Images/viewmodels/ImagesPage.html",
                    Uri = request.Uri
                };

                return page;
            });

            Handle.GET("/images/partials/image/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    var a = new IllustrationPage() {
                        Html = "/Images/viewmodels/ImagePage.html",
                        Data = Db.SQL<Simplified.Ring1.Illustration>("SELECT o FROM Simplified.Ring1.Illustration o WHERE ObjectID=?", objectId).First
                    };

                    return a;
                });
            });

            Handle.GET("/images/partials/concept/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    Something something = Db.SQL<Something>("SELECT o FROM Simplified.Ring1.Something o WHERE ObjectID = ?", objectId).First;
                    ConceptPage a = new ConceptPage() {
                        Html = "/Images/viewmodels/ConceptPage.html",
                        Data = something
                    };

                    return a;
                });
            });

            Handle.GET("/images/partials/concept-somebody/{?}", (string objectId) => {
                return Self.GET("/images/partials/concept/" + objectId);
            });

            Handle.GET("/images/partials/concept-vendible/{?}", (string objectId) => {
                return Self.GET("/images/partials/concept/" + objectId);
            });

            Handle.GET("/images/partials/preview/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    Illustration img = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Illustration;
                    PreviewPage page = new PreviewPage();

                    page.Data = img;

                    return page;
                });
            });

            Handle.GET("/images/partials/preview-chatmessage/{?}", (string objectId) => {
                return Self.GET("/images/partials/preview/" + objectId);
            });

            Handle.GET("/images/partials/preview-chatattachment/{?}", (string objectId) => {
                return Self.GET("/images/partials/preview/" + objectId);
            });

            #region Custom application handlers
            Handle.GET("/images/partials/images-draft/{?}", (string chatMessageId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(chatMessageId));
                var relation = new Illustration
                {
                    Concept = chatMessage
                };
                var draft = new DraftPage
                {
                    SubPage = Self.GET("/images/partials/imagedraftannouncement/" + relation.GetObjectID())
                };
                return draft;
            });

            Handle.GET("/images/partials/images/{?}", (string objectId) => {
                var message = (Something)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                var illustration = Db.SQL<Illustration>(@"Select m from Simplified.Ring1.Illustration m Where m.ToWhat = ?", message).First;
                return illustration == null ? new Page() : Self.GET("/images/partials/preview/" + illustration.GetObjectID());
            });

            Handle.GET("/images/partials/imageattachment/{?}", (string illustrationId) => {
                return Db.Scope<Json>(() =>
                {
                    var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                    if (illustration == null) return new Page();

                    var page = new ConceptIllustrationPage
                    {
                        Html = "/Images/viewmodels/ConceptPage.html"
                    };
                    page.AddNew(illustration);
                    return page;
                });
            });

            Handle.GET("/images/partials/image-edit/{?}", (string illustrationId) => {
                return Db.Scope<Json>(() =>
                {
                    var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                    var page = new ConceptIllustrationPage
                    {
                        Html = "/Images/viewmodels/EditPage.html"
                    };
                    page.AddNew(illustration);
                    return page;
                });
            });

            Handle.GET("/images/partials/image-preview/{?}", (string illustrationId) => {
                return Self.GET("/images/partials/preview/" + illustrationId);
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

            UploadHandlers.GET("/images/images", task => {
                Session.ScheduleTask(task.SessionId, (s, id) => {
                    s.CalculatePatchAndPushOnWebSocket();
                });
            });
            Handle.GET("/images/settings", () =>
            {
                return new Transaction().Scope(() => {
                    var page = new SettingsPage();
                    page.LoadDefaultData();
                    return page;
                });
            }, new HandlerOptions { SelfOnly =  true });
        }

        protected void RegisterMapperHandlers() {

            UriMapping.Map("/images/menu", UriMapping.MappingUriPrefix + "/menu");
            UriMapping.Map("/images/app-name", UriMapping.MappingUriPrefix + "/app-name");
            UriMapping.Map("/images/settings", UriMapping.MappingUriPrefix + "/settings");
            
            UriMapping.OntologyMap("/images/partials/concept-somebody/@w", typeof(Somebody).FullName, null, null);
            UriMapping.OntologyMap("/images/partials/concept-vendible/@w", typeof(Product).FullName, null, null);

            #region Custom application handlers
            UriMapping.OntologyMap("/images/partials/images/@w", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var message = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as ChatMessage;
                return message.IsDraft ? null : objectId;
            });
            UriMapping.OntologyMap("/images/partials/images-draft/@w", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                return chatMessage.IsDraft ? objectId : null;
            });
            UriMapping.OntologyMap("/images/partials/imageattachment/@w", typeof(ChatAttachment).FullName);
            UriMapping.OntologyMap("/images/partials/imagedraftannouncement/@w", typeof(ChatDraftAnnouncement).FullName);
            UriMapping.OntologyMap("/images/partials/imagewarning/@w", typeof(ChatWarning).FullName);

            UriMapping.OntologyMap("/images/partials/image-edit/@w", typeof(EditAnnouncement).FullName);
            UriMapping.OntologyMap("/images/partials/image-preview/@w", typeof(PreviewAnnouncement).FullName);
            #endregion
        }
    }
}
