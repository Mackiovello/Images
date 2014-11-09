using SharedModel;
using Starcounter;

namespace PIMImages {
    partial class EditProduct : Page, IBound<Product> {
        void Handle(Input.ImageUrl action) {
            Data.ImageUrl = action.Value;
        }
    }
}
