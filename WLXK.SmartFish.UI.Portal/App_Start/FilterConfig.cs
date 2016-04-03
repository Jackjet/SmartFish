using System.Web;
using System.Web.Mvc;

namespace WLXK.SmartFish.UI.Portal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}