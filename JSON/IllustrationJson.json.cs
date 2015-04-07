using Starcounter;

namespace Images.JSON {
    partial class IllustrationJson : Page, IBound<Simplified.Ring1.Illustration> {
        void Handle(Input.Delete action) {
            this.ParentPage.ConfirmAction = () => {
                Db.Transact(() => {
                    this.Data.Delete();
                });
            };

            this.ParentPage.Confirm.Message = "Are you sure want to delete image [" + this.Data.Concept.Name + "]?";
        }

        void Handle(Input.Name action) {
            this.Data.Concept = Db.SQL<Simplified.Ring1.Something>("SELECT o FROM Simplified.Ring1.Something o WHERE Name=?", action.Value).First;
        }

        IllustrationsJson ParentPage {
            get {
                return this.Parent.Parent as IllustrationsJson;
            }
        }
    }
}
