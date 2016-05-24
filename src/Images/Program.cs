namespace Images {
    class Program {
        static void Main() {
            var main = new MainHandlers();
            var styles = new DefaultStyles();

            main.Register();
            styles.ApplyIfEmpty();
        }
    }
}
