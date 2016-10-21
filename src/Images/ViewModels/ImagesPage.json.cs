using System;
using Simplified.Ring1;
using Starcounter;

namespace Images
{
    partial class ImagesPage : Json
    {
        public Action ConfirmAction = null;

        public QueryResultRows<Illustration> GetIllustrations => Db.SQL<Illustration>("SELECT o FROM Simplified.Ring1.Illustration o");

        static ImagesPage()
        {
            DefaultTemplate.Illustrations.Bind = nameof(GetIllustrations);
        }

        void Handle(Input.Add action)
        {
            this.RedirectUrl = "images/image/";
        }

        [ImagesPage_json.Confirm]
        partial class PersonsConfirmPage : Page
        {
            void Cancel()
            {
                this.ParentPage.Confirm.Message = null;
                this.ParentPage.ConfirmAction = null;
            }

            void Handle(Input.Reject Action)
            {
                Cancel();
            }

            void Handle(Input.Ok Action)
            {
                if (this.ParentPage.ConfirmAction != null)
                {
                    this.ParentPage.ConfirmAction();
                }

                Cancel();
            }

            public ImagesPage ParentPage
            {
                get
                {
                    return this.Parent as ImagesPage;
                }
            }
        }
    }
}
