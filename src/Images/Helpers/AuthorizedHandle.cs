using System;
using Simplified.Ring3;
using Starcounter;

namespace Images
{
    public class AuthorizedHandle
    {
        public static void GET(string uriTemplate, Func<Response> code, HandlerOptions ho = null)
        {
            Handle.GET(uriTemplate, () => ProceedOrForbid(code));
        }

        public static void GET<T>(string uriTemplate, Func<T, Response> code, HandlerOptions ho = null)
        {
            Handle.GET<T>(uriTemplate, param => {
                return ProceedOrForbid(() => code(param));
            });
        }

        private static Response ProceedOrForbid(Func<Response> code)
        {
            if (IsSignedIn())
            {
                return code();
            }
            else
            {
                return new Response
                {
                    Resource = new UnauthorizedPage(),
                    StatusCode = 403,
                    StatusDescription = "Forbidden"
                };
            }
        }

        public static bool IsSignedIn()
        {
            var user = SystemUser.GetCurrentSystemUser();
            return user != null;
        }
    }
}