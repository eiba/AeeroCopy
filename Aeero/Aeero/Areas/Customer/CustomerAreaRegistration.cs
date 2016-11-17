using System.Web.Mvc;

namespace Aeero.Areas.Customer
{
    public class CustomerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Customer";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Customer_default",
                "Customer/{controller}/{action}/{id}",
                new { area = "Customer", action = "Index", id = UrlParameter.Optional },
                new[] { "Aeero.Areas.Customer.Controllers" }
            );
        }
    }
}