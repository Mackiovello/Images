using Starcounter;
using Simplified.Ring1;

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

            // App name required for Launchpad
            Handle.GET("/images/app-icon", () => {
                return new Page() { Html = "/Images/viewmodels/AppIcon.html" };
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
        }

        protected void RegisterMapperHandlers() {

            UriMapping.Map("/images/menu", UriMapping.MappingUriPrefix + "/menu");
            UriMapping.Map("/images/app-name", UriMapping.MappingUriPrefix + "/app-name");
            UriMapping.Map("/images/app-icon", UriMapping.MappingUriPrefix + "/app-icon");

            UriMapping.OntologyMap("/images/partials/concept-somebody/@w", UriMapping.OntologyMappingUriPrefix + "/concepts.ring1.somebody/@w", null, null);
            UriMapping.OntologyMap("/images/partials/concept-vendible/@w", UriMapping.OntologyMappingUriPrefix + "/concepts.ring2.vendible/@w", null, null);

            UriMapping.OntologyMap("/images/partials/preview/@w", UriMapping.OntologyMappingUriPrefix + "/simplified.ring6.chatattachment/@w", (string objectId) => {
                return objectId;
            }, (string objectId) => {
                Relation rel = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Relation;

                if (rel.WhatIs != null && rel.WhatIs.GetType() == typeof(Illustration)) {
                    return rel.WhatIs.Key;
                }

                return null;
            });
        }
    }
}
