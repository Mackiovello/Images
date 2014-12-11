using Concepts.Ring1;
using Starcounter;

namespace Image.JSON {
    partial class IllustrationJson : Page, IBound<Illustration> {

        protected override void OnData() {
            base.OnData();
            
            //this.Transaction = new Transaction(false, false);
        }

        protected override void HasChanged(Starcounter.Templates.TValue property) {
            base.HasChanged(property);

            if (this.Transaction != null) {
            //    this.Transaction.Commit();
            }
        }

        void Handle(Input.Delete action) {

        
            ((Concepts.Ring1.Illustration)this.Data).Delete();
            this.Transaction.Commit();

     
        }

        void Handle(Input.Name i) {
            var o = Db.SQL<Something>("SELECT o FROM Something o WHERE Name=?", i.Value).First;
            this.Data.Concept = o;
        }


    }
}
