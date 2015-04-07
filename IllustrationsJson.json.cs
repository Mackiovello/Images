using System;
using Starcounter;

namespace Images {
    partial class IllustrationsJson : Json {
        public Action ConfirmAction = null;

        public QueryResultRows<Simplified.Ring1.Illustration> _Illustrations {
            get {
                return Db.SQL<Simplified.Ring1.Illustration>("SELECT o FROM Simplified.Ring1.Illustration o");
            }
        }

        void Handle(Input.Add action) {
            Db.Transact(() => {
                Simplified.Ring1.Illustration i = new Simplified.Ring1.Illustration();
                i.Concept = new Simplified.Ring1.Something();
                i.Content = new Simplified.Ring1.Content();
            });
        }
        
        public override string GetHtmlPartialUrl() {
            return Html;
        }

        [IllustrationsJson_json.Confirm]
        partial class PersonsConfirmPage : Page {
            void Cancel() {
                this.ParentPage.Confirm.Message = null;
                this.ParentPage.ConfirmAction = null;
            }

            void Handle(Input.Reject Action) {
                Cancel();
            }

            void Handle(Input.Ok Action) {
                if (this.ParentPage.ConfirmAction != null) {
                    this.ParentPage.ConfirmAction();
                }

                Cancel();
            }

            public IllustrationsJson ParentPage {
                get {
                    return this.Parent as IllustrationsJson;
                }
            }
        }
    }
}
