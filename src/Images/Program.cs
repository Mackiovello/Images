namespace Images
{
    class Program
    {
        static void Main()
        {
            var main = new MainHandlers();
            main.Register();

            var upgrade = new UpgradeData();
            upgrade.UpgradeMimeType();
        }
    }
}