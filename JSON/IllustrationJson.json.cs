using Simplified.Ring1;
using Starcounter;

namespace Images.JSON {
    partial class IllustrationJson : Page, IBound<Simplified.Ring1.Illustration> {
        protected override void OnData() {
            base.OnData();

            this.MaxFileSize = Image.ImageUpload.MaxFileSize;
            this.AllowedMimeTypes = string.Join(",", Image.ImageUpload.AllowedMimeTypes);

            if (this.Data == null) {
                Illustration i = new Illustration();

                i.Concept = new Something() { Name = "Standalone image" };
                i.Content = new Content();

                this.Data = i;
            }
        }

        void Handle(Input.Delete Action) {
            this.ParentPage.ConfirmAction = () => {
                Db.Transact(() => {
                    this.Data.Delete();
                });
            };

            this.ParentPage.Confirm.Message = "Are you sure want to delete image [" + this.Data.Concept.Name + "]?";
        }

        void Handle(Input.Name Action) {
            this.Data.Concept = Db.SQL<Simplified.Ring1.Something>("SELECT o FROM Simplified.Ring1.Something o WHERE Name = ?", Action.Value).First;
        }

        void Handle(Input.Cancel Action) {
            this.RedirectUrl = "/Images";
        }

        void Handle(Input.Save Action) {
            this.Transaction.Commit();
            this.RedirectUrl = "/Images";
        }

        IllustrationsJson ParentPage {
            get {
                return this.Parent.Parent as IllustrationsJson;
            }
        }
    }
}
