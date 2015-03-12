using Starcounter;

namespace Images.JSON {
    partial class IllustrationJson : Page, IBound<Simplified.Ring1.Illustration> {

        void Handle(Input.Delete action) {

            this.Data.Delete();
            this.Transaction.Commit();
        }

        void Handle(Input.Name action) {
            this.Data.Concept = Db.SQL<Simplified.Ring1.Something>("SELECT o FROM Simplified.Ring1.Something o WHERE Name=?", action.Value).First;
        }
    }
}
