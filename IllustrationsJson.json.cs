using Concepts.Ring1;
using Starcounter;

namespace Images {
    partial class IllustrationsJson : Json {

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
