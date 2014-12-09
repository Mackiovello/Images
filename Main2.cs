using Concepts.Ring8;
using Starcounter;

namespace Image {
    class Startup {
        static void Main() {
            Handle.GET("/Image/product/{?}", (string objectId) => {
                return X.GET<EditProduct>("/Image/partials/product/" + objectId);
            });

            Handle.GET("/Image/partials/product/{?}", (string objectId) => {
                return Db.Scope<Json>(() => {
                    return new EditProduct() { 
                        Data = Product.FindByOID(objectId)
                    };
                });
            });

            RegisterMapperHandlers();
            ImageUpload.RegisterHandlers();
        }

        private static void RegisterMapperHandlers() {

            Handle.GET("/Image/partials/product/{?}", (string objectId) => {
                return (Json)X.GET("/sharedmodel/product/" + objectId);
            }, HandlerOptions.DefaultLevel);

            Handle.GET("/sharedmodel/product/{?}", (string objectId) => {
                return (Json)X.GET("/Image/partials/product/" + objectId, 0, HandlerOptions.ApplicationLevel);
            }, HandlerOptions.DefaultLevel);
        }
    }
}
