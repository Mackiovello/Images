using Starcounter;
using Simplified.Ring1;
using Simplified.Ring3;
using Simplified.Ring6;
using Partial = Images.Helpers.Partial;

namespace Images {
    internal class MainHandlers {
        public void Register() {
            Partial.Register("/images/standalone", () => {
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
            Partial.Register("/images", (Request request) => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.CurrentPage = Partial.Get("/images/partials/images");

                    return master;
                });
            });

            Partial.Register("/images/image/{?}", (string objectId) => {
                return Db.Scope<StandalonePage>(() => {
                    StandalonePage master = this.GetMaster();

                    master.CurrentPage = Partial.Get<IllustrationPage>("/Images/partials/image/" + objectId);

                    return master;
                });
            });

            Partial.Register("/images/concept/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    return Partial.Get<ConceptPage>("/images/partials/concept/" + objectId);
                });
            });

            this.RegisterPartials();
            this.RegisterLauncherHooks();
            this.RegisterMapperHandlers();

            IllustrationHelper helper = new IllustrationHelper();

            helper.DeleteOldFiles();
        }

        protected StandalonePage GetMaster() {
            return Partial.Get<StandalonePage>("/images/standalone");
        }

        protected void RegisterLauncherHooks() {
            Partial.Register("/images/app-name", () => {
                return new AppName();
            });

            // Menu
            Partial.Register("/images/menu", () => {
                return new Page() { Html = "/Images/viewmodels/AppMenu.html" };
            });
        }

        protected void RegisterPartials() {
            Partial.Register("/images/partials/images", (Request request) => {
                IllustrationsPage page = new IllustrationsPage() {
                    Html = "/Images/viewmodels/ImagesPage.html",
                    Uri = request.Uri
                };

                return page;
            });

            Partial.Register("/images/partials/image/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    var a = new IllustrationPage() {
                        Html = "/Images/viewmodels/ImagePage.html",
                        Data = Db.SQL<Simplified.Ring1.Illustration>("SELECT o FROM Simplified.Ring1.Illustration o WHERE ObjectID=?", objectId).First
                    };

                    return a;
                });
            });

            Partial.Register("/images/partials/concept/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    Something something = Db.SQL<Something>("SELECT o FROM Simplified.Ring1.Something o WHERE ObjectID = ?", objectId).First;
                    ConceptPage a = new ConceptPage() {
                        Html = "/Images/viewmodels/ConceptPage.html",
                        Data = something
                    };

                    return a;
                });
            });

            Partial.Register("/images/partials/concept-somebody/{?}", (string objectId) => {
                return Partial.Get("/images/partials/concept/" + objectId);
            });

            Partial.Register("/images/partials/concept-vendible/{?}", (string objectId) => {
                return Partial.Get("/images/partials/concept/" + objectId);
            });

            Partial.Register("/images/partials/concept-chatgroup/{?}", (string objectId) => {
                return Partial.Get("/images/partials/concept/" + objectId);
            });

            Partial.Register("/images/partials/preview/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    Illustration img = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Illustration;
                    PreviewPage page = new PreviewPage();

                    page.Data = img;

                    return page;
                });
            });

            Partial.Register("/images/partials/preview-chatmessage/{?}", (string objectId) => {
                return Partial.Get("/images/partials/preview/" + objectId);
            });

            Partial.Register("/images/partials/preview-chatattachment/{?}", (string objectId) => {
                return Partial.Get("/images/partials/preview/" + objectId);
            });
        }

        protected void RegisterMapperHandlers() {

            Partial.Map("/images/menu", Partial.MappingUriPrefix + "/menu");
            Partial.Map("/images/app-name", Partial.MappingUriPrefix + "/app-name");

            Partial.Map("/images/partials/concept-chatgroup/@w", typeof(ChatGroup).FullName, null, null);
            Partial.Map("/images/partials/concept-somebody/@w", typeof(Somebody).FullName, null, null);
            Partial.Map("/images/partials/concept-vendible/@w", typeof(Product).FullName, null, null);

            Partial.Map("/images/partials/preview-chatmessage/@w", typeof(ChatMessage).FullName, null, (string objectId) => {
                var illustration = Db.SQL<Simplified.Ring1.Illustration>("SELECT i FROM Simplified.Ring1.Illustration i WHERE i.Concept.Key = ?", objectId).First;

                return illustration != null ? illustration.Key : null;
            });

            Partial.Map("/images/partials/preview-chatattachment/@w", typeof(ChatAttachment).FullName, (string objectId) => {
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
