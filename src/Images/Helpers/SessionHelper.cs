using System;
using Starcounter;

namespace Images
{
    public class SessionHelper
    {
        public static MasterPage GetMaster()
        {
            Session session = Session.Current ?? new Session(SessionOptions.PatchVersioning);

            return session?.Data as MasterPage
                   ?? new MasterPage
                   {
                       Session = session
                   };
        }

        public static MasterPage GetMaster(Func<Json> getPartial)
        {
            if (getPartial == null) throw new ArgumentNullException(nameof(getPartial));

            var master = GetMaster();
            master.CurrentPage = getPartial();
            return master;
        }
    }
}