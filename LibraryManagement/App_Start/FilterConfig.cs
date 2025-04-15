using System.Web;
using System.Web.Mvc;
using LibraryManagement.Filters;

namespace LibraryManagement
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SessionAuthFilters()); 
        }
    }
}