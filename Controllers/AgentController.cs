using Microsoft.AspNetCore.Mvc;

namespace RealEstatePortal.Controllers
{
    public class AgentController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
