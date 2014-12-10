using Concepts.Ring1;
using Concepts.Ring8;
using Image.JSON;
using PolyjuiceNamespace;
using Starcounter;

namespace Image {
    class Startup {
        static void Main() {
            Handle.GET("/Image/image/{?}", (string objectId) => {
                return X.GET<IllustrationJson>("/Image/partials/image/" + objectId);
            });

            Handle.GET("/Image/partials/image/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    var a = new IllustrationJson() { 
                        Html = "/Image/image.html",
                        Data = Db.SQL<Illustration>("SELECT o FROM Illustration o WHERE ObjectID=?", objectId ).First
                    };

                    return a;

                });
            });

            Starcounter.Handle.GET("/launcher/app-name", () => {
                return new AppName();
            }, HandlerOptions.ApplicationLevel);

            // App name required for Launchpad
            Starcounter.Handle.GET("/launcher/app-icon", () => {
                return new Page() { Html = "/Image/app-icon.html" };
            }, HandlerOptions.ApplicationLevel);

            // Menu
            Starcounter.Handle.GET("/launcher/menu", () => {
                return new Page() { Html = "/Image/app-menu.html" };
            }, HandlerOptions.ApplicationLevel);


            // Workspace root (Launchpad)
            Starcounter.Handle.GET("/Image", (Request request) => {

                return new Images() {
                    Html = "/Image/images.html", Uri = request.Uri
                };
            });

            RegisterMapperHandlers();
            ImageUpload.RegisterHandlers();
        }

        private static void RegisterMapperHandlers() {
            //Polyjuice.Map("/Image/partials/image/@w", "/so/illustration/@w",null,null);
        }
    }
}
