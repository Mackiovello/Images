using Concepts.Ring1;
using Starcounter;

namespace Image {
    partial class Images : Json {

        public QueryResultRows<Illustration> _Illustrations {

            get {
                return Db.SQL<Illustration>("SELECT o FROM Illustration o");
            }
        }

        void Handle(Input.Add action) {


            Db.Transaction(() => {
                Illustration i = new Illustration();
                i.Concept = new PhysicalObject();    // TODO:
                i.Content = new Content();
            });

//            Illustrations.Add();

        }


        public override string GetHtmlPartialUrl() {
            return Html;
        }
  
    }
}
