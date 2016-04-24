using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simplified.Ring1;
using Starcounter;

namespace Images.Api
{
    internal class CommitHooks
    {
        public void Register()
        {
            Hook<Illustration>.CommitInsert += (s, a) => {
                if (a.Content == null)
                {
                    a.Delete();
                } else if (string.IsNullOrEmpty(a.Content.URL))
                {
                    a.Delete();
                }
            };
        }
    }
}
