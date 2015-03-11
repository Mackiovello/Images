using Concepts.Ring1;
using Concepts.Ring8;
using Image;
using Images.JSON;
using PolyjuiceNamespace;
using Starcounter;

namespace Images {
    class Startup {
        static void Main() {
            Handle.GET("/Images/image/{?}", (string objectId) => {
                return X.GET<IllustrationJson>("/Images/partials/image/" + objectId);
            });

            Handle.GET("/Images/partials/image/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    var a = new IllustrationJson() {
                        Html = "/Images/image.html",
                        Data = Db.SQL<Illustration>("SELECT o FROM Illustration o WHERE ObjectID=?", objectId ).First
                    };

                    return a;

                });
            });

            Handle.GET("/Images/partials/concept/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    var a = new ConceptJson() {
                        Html = "/Images/concept.html",
                        Data = Db.SQL<Something>("SELECT o FROM Something o WHERE ObjectID=?", objectId).First
                    };

                    return a;

                });
            });

            Handle.GET("/Images/concept/{?}", (string objectId) => {
                return X.GET<ConceptJson>("/Image/partials/concept/" + objectId);
            });




            Starcounter.Handle.GET("/Images/app-name", () => {
                return new AppName();
            });

            // App name required for Launchpad
            Starcounter.Handle.GET("/Images/app-icon", () => {
                return new Page() { Html = "/Images/app-icon.html" };
            });

            //// Menu
            //Starcounter.Handle.GET("/Images/menu", () => {
            //    return new Page() { Html = "/Images/app-menu.html" };
            //});


            // Workspace root (Launchpad)
            Starcounter.Handle.GET("/Images", (Request request) => {
              return Db.Scope<IllustrationsJson>(() => {
                return new IllustrationsJson() {
                  Html = "/Images/images.html",
                  Uri = request.Uri
                };
              });
            });

            RegisterMapperHandlers();
            ImageUpload.RegisterHandlers();
        }

        private static void RegisterMapperHandlers() {
//            Polyjuice.Map("/Images/menu", "/polyjuice/menu");
            Polyjuice.Map("/Images/app-name", "/polyjuice/app-name");
            Polyjuice.Map("/Images/app-icon", "/polyjuice/app-icon");
            Polyjuice.OntologyMap("/Images/partials/concept/@w", "/so/something/@w", null, null);
        }
    }
}
