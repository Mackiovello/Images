using Concepts.Ring8;
using Starcounter;

namespace Image {
    partial class EditProduct : Page, IBound<Product> {
        void Handle(Input.ImageUrl action) {
            Data.ImageUrl = action.Value;
        }
    }
}
