using Concepts.Ring1;
using Starcounter;

namespace Images.JSON {
    partial class ConceptJson : Page, IBound<Something> {

        protected override void OnData() {
            base.OnData();
            if (this.Data.Illustration == null) {
                var i = new Illustration();
                i.Content = new WebContent();
                i.Concept = this.Data;
                var test = this.Data.Illustration;
            }
        }
    }
}
