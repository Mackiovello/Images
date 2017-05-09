using System.Collections.Generic;

namespace Images
{
    public interface IPageRouting
    {
        List<RoutingPreset> GetRoutingPresets();
    }
}