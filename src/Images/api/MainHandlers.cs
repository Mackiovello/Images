using PolyjuiceNamespace;
using Starcounter;

namespace Images {
    internal class MainHandlers {
        public void Register() {
            Handle.GET("/images/image/{?}", (string objectId) => {
                return Self.GET<IllustrationPage>("/Images/partials/image/" + objectId);
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
                    ConceptPage a = new ConceptPage() {
                        Html = "/Images/viewmodels/ConceptPage.html",
                        Data = Db.SQL<Simplified.Ring1.Something>("SELECT o FROM Simplified.Ring1.Something o WHERE ObjectID=?", objectId).First
                    };

                    return a;
                });
            });

            Handle.GET("/images/concept/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    return Self.GET<ConceptPage>("/images/partials/concept/" + objectId);
                });
            });

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


            // Workspace root (Launchpad)
            Handle.GET("/images", (Request request) => {
                return Db.Scope<IllustrationsPage>(() => {
                    return new IllustrationsPage() {
                        Html = "/Images/viewmodels/ImagesPage.html",
                        Uri = request.Uri
                    };
                });
            });

            this.RegisterMapperHandlers();
        }

        protected void RegisterMapperHandlers() {
            Polyjuice.Map("/images/menu", "/polyjuice/menu");
            Polyjuice.Map("/images/app-name", "/polyjuice/app-name");
            Polyjuice.Map("/images/app-icon", "/polyjuice/app-icon");
            Polyjuice.OntologyMap("/images/partials/concept/@w", "/so/something/@w", null, null);
        }
    }
}
