using Microsoft.AspNetCore.Mvc;

namespace RealEstatePortal.Controllers
{
    public class AdminController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
