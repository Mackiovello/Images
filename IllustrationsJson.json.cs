using System;
using Simplified.Ring1;
using Starcounter;

namespace Images {
    partial class IllustrationsJson : Json {
        public Action ConfirmAction = null;

        public QueryResultRows<Illustration> _Illustrations {
            get {
                return Db.SQL<Illustration>("SELECT o FROM Simplified.Ring1.Illustration o");
            }
        }

        void Handle(Input.Add action) {
            this.RedirectUrl = "Images/image/";
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
