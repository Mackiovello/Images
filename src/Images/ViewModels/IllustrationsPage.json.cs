using System;
using Simplified.Ring1;
using Starcounter;

namespace Images {
    partial class IllustrationsPage : Json {
        public Action ConfirmAction = null;

        public QueryResultRows<Illustration> _Illustrations {
            get {
                return Db.SQL<Illustration>("SELECT o FROM Simplified.Ring1.Illustration o");
            }
        }

        void Handle(Input.Add action) {
            this.RedirectUrl = "images/image/";
        }
        
        [IllustrationsPage_json.Confirm]
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

            public IllustrationsPage ParentPage {
                get {
                    return this.Parent as IllustrationsPage;
                }
            }
        }
    }
}
