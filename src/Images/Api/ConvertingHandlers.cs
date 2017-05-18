using System;
using Starcounter;
using Simplified.Ring1;

namespace Images
{
    class ConvertingHandlers
    {
        public void Register()
        {
            CreateConvertingHandler("/images/partials/somethings-single/{?}", "/images/partials/illustrations/",
                (string somethingId) =>
                {
                    var something = DbHelper.FromID(DbHelper.Base64DecodeObjectID(somethingId)) as Something;
                    var illustration = Db.SQL<Illustration>(@"Select m from Simplified.Ring1.Illustration m Where m.ToWhat = ?", something).First;
                    return illustration?.Key;
                },
                () => new Page());

            CreateConvertingHandler("/images/partials/illustrations/{?}", "/images/partials/contents/",
                (string illustrationId) =>
                {
                    var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                    return illustration?.Content.Key;
                },
                () => new ErrorPage { ErrorText = "Images cannot present an illustration without content" });

            CreateConvertingHandler("/images/partials/illustrations-edit/{?}", "/images/partials/contents-edit/",
                (string illustrationId) =>
                {
                    var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                    if (illustration != null && illustration.Content == null)
                    {
                        illustration.Content = new Content { Name = "Standalone image" };
                    }
                    return illustration?.Content.Key;
                },
                () => new ErrorPage { ErrorText = "Images cannot present an empty illustration" });
        }

        private void CreateConvertingHandler(string fromUri, string toUri, Func<string, string> ruleFn, Func<Response> errorFn)
        {
            Handle.GET(fromUri, (string fromObjectId) =>
            {
                var toObjectId = ruleFn(fromObjectId);
                if (toObjectId == null)
                {
                    return errorFn();
                }
                return Self.GET(toUri + toObjectId);
            });

            var fromNonPartialUri = fromUri.Replace("/partials", "");
            var toNonPartialUri = toUri.Replace("/partials", "");

            Handle.GET(fromNonPartialUri, (string fromObjectId) =>
            {
                var toObjectId = ruleFn(fromObjectId);
                if (toObjectId == null)
                {
                    return errorFn();
                }
                return Self.GET(toNonPartialUri + toObjectId);
            });
        }
    }
}
