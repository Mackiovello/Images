using PolyjuiceNamespace;
using Starcounter;

namespace Images {
    class Program {
        static void Main() {
            UploadHandlers upload = new UploadHandlers();
            MainHandlers main = new MainHandlers();

            main.Register();
            upload.Register();
        }
    }
}
