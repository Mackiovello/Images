using System;
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

            //For TextPage similar in Images, People etc.
            Handle.GET("/images/partials/chatattachmentimage/{?}", (string chatMessageDraftId) =>
            {
                //Do that because ontology mapping support just 1 parameter
                var path = chatMessageDraftId + " image";
                var draft = Self.GET("/images/partials/imagesdraftannouncement/" + Base64Encode(path));
                return draft;
            });
            Handle.GET("/images/partials/concept-chatmessage/{?}", (string objectId) => {
                return Self.GET("/images/partials/concept/" + objectId);
            });
            Handle.GET("/images/partials/imagesdraftannouncement/{?}", (string objectPath) => null);
        }


        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        private string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        protected void RegisterMapperHandlers() {

            UriMapping.Map("/images/menu", UriMapping.MappingUriPrefix + "/menu");
            UriMapping.Map("/images/app-name", UriMapping.MappingUriPrefix + "/app-name");
            UriMapping.OntologyMap("/images/partials/concept-somebody/@w", typeof(Somebody).FullName, null, null);
            UriMapping.OntologyMap("/images/partials/concept-vendible/@w", typeof(Product).FullName, null, null);

            //UriMapping.OntologyMap("/images/partials/preview-chatattachment/@w", typeof(ChatMessage).FullName, (string objectId) => {
            //    return objectId;
            //}, (string objectId) => {
            //    Relation rel = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Relation;

            //    if (rel.WhatIs != null && rel.WhatIs.GetType() == typeof(Illustration)) {
            //        return rel.WhatIs.Key;
            //    }

            //    return null;
            //});

            //For TextPage similar in Images, People etc.
            UriMapping.OntologyMap("/images/partials/chatattachmentimage/@w", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var message = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                return message.IsDraft ? objectId : null;
            });
            UriMapping.OntologyMap("/images/partials/concept-chatmessage/@w", typeof(ChatAttachment).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var data = Base64Decode(objectId).Split(' ');
                if (data[1] == "image")
                {
                    return data[1];
                }
                return null;
            });
            UriMapping.OntologyMap("/images/partials/imagesdraftannouncement/@w", typeof(ChatDraftAnnouncement).FullName, objectId => objectId, objectId => null);
            
        }
    }
}
