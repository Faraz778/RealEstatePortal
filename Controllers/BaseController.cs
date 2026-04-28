using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace RealEstatePortal.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var email = HttpContext.Session.GetString("UserEmail");

            System.Diagnostics.Debug.WriteLine("SESSION EMAIL: " + email);

            if (string.IsNullOrEmpty(email))
            {
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}