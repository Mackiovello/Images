using Concepts.Ring8;
using PolyjuiceNamespace;
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
            Polyjuice.Map("/Image/partials/product/@w", "/so/product/@w");
        }
    }
}
