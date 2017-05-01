using Starcounter;

namespace Images
{
    public class SessionHelper
    {
        public static MasterPage GetMaster()
        {
            var session = Session.Current ?? new Session(SessionOptions.PatchVersioning);

            var master = session?.Data as MasterPage;
            if (master != null)
            {
                return master;
            }

            return new MasterPage
            {
                Session = session
            };
        }
    }
}