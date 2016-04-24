    using Images.Api;

namespace Images {
    class Program {
        static void Main() {
            UploadHandlers upload = new UploadHandlers();
            MainHandlers main = new MainHandlers();
            DefaultStyles styles = new DefaultStyles();
            CommitHooks hooks = new CommitHooks();

            main.Register();
            upload.Register();
            styles.ApplyIfEmpty();
            hooks.Register();
        }
    }
}
