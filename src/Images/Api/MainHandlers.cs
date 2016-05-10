using Images.Helpers;
using Starcounter;
using Simplified.Ring1;
using Simplified.Ring3;
using Simplified.Ring6;

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
            Handle.GET("/images/partials/chatattachmentimage/{?}", (string chatMessageId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(chatMessageId));
                var content = new Content();
                var relation = new Illustration
                {
                    Content = content,
                    Concept = chatMessage
                };
                var draft = Self.GET("/images/partials/imagedraftannouncement/" + relation.GetObjectID());
                return draft;
            });

            Handle.GET("/images/partials/chatmessageimage/{?}", (string contentId) => {
                return Db.Transact<Json>(() => {
                    var page = new ConceptIllustrationPage
                    {
                        Html = "/Images/viewmodels/ConceptPage.html"
                    };
                    page.RefreshData(contentId);
                    return page;
                });
            });

            Handle.GET("/images/partials/chatattachmentimagepreview/{?}", (string illustrationId) => {
                return Self.GET("/images/partials/preview/" + illustrationId);
            });

            Handle.GET("/images/partials/imagedraftannouncement/{?}", (string objectPath) => null);

            Handle.GET("/images/partials/imagewarning/{?}", (string contentId) =>
            {
                var page = new ConceptIllustrationWarningPage();
                page.RefreshData(contentId);
                return page;
            });
            #endregion
        }

        protected void RegisterMapperHandlers() {

            UriMapping.Map("/images/menu", UriMapping.MappingUriPrefix + "/menu");
            UriMapping.Map("/images/app-name", UriMapping.MappingUriPrefix + "/app-name");
            UriMapping.OntologyMap("/images/partials/concept-somebody/@w", typeof(Somebody).FullName, null, null);
            UriMapping.OntologyMap("/images/partials/concept-vendible/@w", typeof(Product).FullName, null, null);

            #region Custom application handlers
            UriMapping.OntologyMap("/images/partials/chatattachmentimage/@w", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                return chatMessage.IsDraft ? objectId : null;
            });
            UriMapping.OntologyMap("/images/partials/chatattachmentimagepreview/@w", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                if (chatMessage.IsDraft) return null;

                var illustration = Db.SQL<Illustration>(@"Select m from Simplified.Ring1.Illustration m Where m.WhatIs = ?", chatMessage).First;
                return illustration?.GetObjectID();
            });
            UriMapping.OntologyMap("/images/partials/chatmessageimage/@w", typeof(ChatAttachment).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Illustration;
                return illustration?.Content?.GetObjectID();
            });
            UriMapping.OntologyMap("/images/partials/imagedraftannouncement/@w", typeof(ChatDraftAnnouncement).FullName, objectId => objectId, objectId => null);
            UriMapping.OntologyMap("/images/partials/imagewarning/@w", typeof(ChatWarning).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Illustration;
                if (illustration?.Content == null) return null;

                var result = ImageValidator.IsValid(illustration.Content);
                return string.IsNullOrEmpty(result) ? null : illustration.Content.GetObjectID();
            });
            #endregion
        }
    }
}
