using PolyjuiceNamespace;
using Starcounter;

namespace Images {
    class Program {
        static void Main() {
            UploadHandlers upload = new UploadHandlers();
            MainHandlers main = new MainHandlers();
            DefaultStyles styles = new DefaultStyles();

            main.Register();
            upload.Register();
            styles.ApplyIfEmpty();
        }
    }
}
