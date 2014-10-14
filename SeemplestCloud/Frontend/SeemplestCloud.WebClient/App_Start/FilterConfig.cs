using System.Web.Mvc;
using SeemplestCloud.WebClient.Infrastructure;

namespace SeemplestCloud.WebClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
