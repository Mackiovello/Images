using SharedModel;
using Starcounter;

namespace PIMImages {
    class Startup {
        static void Main() {
            Handle.GET("/pimimages/product/{?}", (string objectId) => {
                return X.GET<EditProduct>("/pimimages/partials/product/" + objectId);
            });

            Handle.GET("/pimimages/partials/product/{?}", (string objectId) => {
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

            Handle.GET("/pimimages/partials/product/{?}", (string objectId) => {
                return (Json)X.GET("/sharedmodel/product/" + objectId);
            }, HandlerOptions.DefaultLevel);

            Handle.GET("/sharedmodel/product/{?}", (string objectId) => {
                return (Json)X.GET("/pimimages/partials/product/" + objectId, 0, HandlerOptions.ApplicationLevel);
            }, HandlerOptions.DefaultLevel);
        }
    }
}
