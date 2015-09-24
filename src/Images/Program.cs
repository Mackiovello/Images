using Starcounter;

namespace Images {
    class Program {
        static void Main() {
            UploadHandlers upload = new UploadHandlers();
            MainHandlers main = new MainHandlers();
            DefaultStyles styles = new DefaultStyles();

            main.Register();
            upload.Register();

            Handle.GET("/Images/ApplyLayouts", () => {
                styles.ApplyIfEmpty();
                return 200;
            });

            UriMapping.Map("/Images/ApplyLayouts", UriMapping.MappingUriPrefix + "/user");
        }
    }
}
