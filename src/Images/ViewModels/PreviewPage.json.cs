using Starcounter;

namespace Images {
    partial class PreviewPage : Page, IBound<Simplified.Ring1.Illustration> {
        protected override void OnData() {
            base.OnData();
            this.Url = string.Format("/images/image/{0}", this.Key);
        }

        public void Handle(Input.ShowLightBox action)
        {
            IsLightBoxVisible = true;
        }
    }
}
