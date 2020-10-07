using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RentACarMVC.Models;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RentACarMVC.Classes;

namespace RentACarMVC
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            CreateRolesAndSuperUser();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void CreateRolesAndSuperUser()
        {
            UssersHelper.CheckRole("Admin");
            UssersHelper.CheckRole("Cliente");
            UssersHelper.CheckRole("Cajero");
            UssersHelper.CheckRole("Chofer");
            UssersHelper.CheckRole("Borrame3");
            UssersHelper.CheckSuperUser();
        }
    }
}
