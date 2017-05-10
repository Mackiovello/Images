using System.Collections.Generic;
using Simplified.Ring1;
using Starcounter;

namespace Images
{
    public class ConceptIllustrationWarningPageRouting : IPageRouting
    {
        public List<RoutingPreset> GetRoutingPresets()
        {
            return new List<RoutingPreset>
            {
                new RoutingPreset("/images/partials/imagewarning/{?}",
                    info =>
                    {
                        var illustrationId = info.Arguments[0];
                        var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                        if (illustration == null) return new Page();

                        var page = new ConceptIllustrationWarningPage();
                        page.RefreshData(illustration);
                        return page;
                    }
                )
            };
        }
    }
}