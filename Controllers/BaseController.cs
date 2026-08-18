
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using RealEstatePortal.Data;

namespace RealEstatePortal.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BaseController(ApplicationDbContext db)
        {
            _db = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(email))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var controller = context.RouteData.Values["controller"]?.ToString();

            //  ADMIN ONLY
            if (controller == "Admin" && role != "Admin")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            // AGENT ONLY
            if (controller == "Agent" && role != "Agent")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            //  PRIVATE SELLER ONLY
            if (controller == "PrivateSeller" && role != "PrivateSeller")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                return;
            }

            // LOAD MESSAGES FOR ADMIN LAYOUT DROPDOWN
            if (controller == "Admin")
            {
                ViewBag.Messages = _db.ContactMessages
                    .OrderByDescending(x => x.SentDate)
                    .Take(5)
                    .ToList();
           

                ViewBag.UnreadCount = _db.ContactMessages
                    .Count(x => !x.IsRead);
            }

            base.OnActionExecuting(context);
        }
    }
}

