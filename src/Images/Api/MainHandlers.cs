using PolyjuiceNamespace;
using Starcounter;

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

                    IllustrationsPage page = new IllustrationsPage() {
                        Html = "/Images/viewmodels/ImagesPage.html",
                        Uri = request.Uri
                    };

                    master.CurrentPage = page;

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
                    ConceptPage a = new ConceptPage() {
                        Html = "/Images/viewmodels/ConceptPage.html",
                        Data = Db.SQL<Simplified.Ring1.Something>("SELECT o FROM Simplified.Ring1.Something o WHERE ObjectID=?", objectId).First
                    };

                    return a;
                });
            });
        }

        protected void RegisterMapperHandlers() {
            Polyjuice.Map("/images/menu", "/polyjuice/menu");
            Polyjuice.Map("/images/app-name", "/polyjuice/app-name");
            Polyjuice.Map("/images/app-icon", "/polyjuice/app-icon");
            Polyjuice.OntologyMap("/images/partials/concept/@w", "/so/something/@w", null, null);
        }
    }
}
