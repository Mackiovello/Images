using Concepts.Ring1;
using Starcounter;

namespace Images.JSON {
    partial class ConceptJson : Page, IBound<Something> {


        //protected override void OnData() {
        //    base.OnData();
        //    if (this.Data != null && this.Data.Illustration == null) {
        //        var i = new Illustration();
        //        i.Content = new WebContent();
        //        i.Concept = this.Data;
        //        var test = this.Data.Illustration;
        //    }
        //}

        public string URL {

            get {

                if (this.Data.Illustration != null && this.Data.Illustration.Content is WebContent) {
                    return this.Data.Illustration.Content.URL;
                }
                return null;
            }
            set {

                Illustration illustration = this.Data.Illustration;
                if (illustration == null) {
                    illustration = new Illustration();
                    illustration.Content = new WebContent() { URL = value };
                    illustration.Concept = this.Data;
                }
                else {

                    if (illustration.Content is WebContent) {
                        ((WebContent)illustration.Content).URL = value;
                    }
                }
            }
        }

        void Handle(Input.Delete action) {

            if (this.Data.Illustration != null) {
                if (this.Data.Illustration.Content != null) {
                    //if (this.Data.Illustration.Content is WebContent) {
                    //    ((WebContent)this.Data.Illustration.Content).URL = null;
                    //}
                    this.Data.Illustration.Content.Delete();
                }
                this.Data.Illustration.Delete();
            }
            //            this.Transaction.Commit();
        }
    }

}
