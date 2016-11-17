using System.Web.Mvc;

namespace Aeero.Areas.Touch
{
    public class TouchAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Touch";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Touch_default",
                "Touch/{controller}/{action}/{id}",
                new { area="Touch", action = "Index", id = UrlParameter.Optional },
                new[] { "Aeero.Areas.Touch.Controllers" }
            );
        }
    }
}