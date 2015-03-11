using Concepts.Ring1;
using Starcounter;

namespace Images.JSON {
    partial class IllustrationJson : Page, IBound<Illustration> {

        void Handle(Input.Delete action) {

            this.Data.Delete();
            this.Transaction.Commit();
        }

        void Handle(Input.Name action) {
            this.Data.Concept = Db.SQL<Something>("SELECT o FROM Something o WHERE Name=?", action.Value).First;
        }
    }
}
