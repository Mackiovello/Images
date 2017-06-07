namespace Images
{
    class Program
    {
        static void Main()
        {
            var authorizedHandlers = new AuthorizedHandlers();
            authorizedHandlers.Register();

            var main = new MainHandlers();
            main.Register();

            var converting = new ConvertingHandlers();
            converting.Register();

            var upgrade = new UpgradeData();
            upgrade.UpgradeMimeType();
        }
    }
}