using Microsoft.AspNetCore.Mvc;
using RealEstatePortal.Data;
using RealEstatePortal.Models;

namespace RealEstatePortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Register Page
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register User
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login","Home");
            }

            return View(user);
        }

        // GET: Login Page (we will build next)
        public IActionResult Login()
        {
            //return View();
            return View("~/Views/Home/Login.cshtml");

        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (existingUser != null)
            {
                // SESSION (we add now 🔥)
                HttpContext.Session.SetString("UserId", existingUser.Id.ToString()); // ✅ ADD THIS

                HttpContext.Session.SetString("UserEmail", existingUser.Email);
                HttpContext.Session.SetString("UserRole", existingUser.Role);
                HttpContext.Session.SetString("UserName", existingUser.Name); // ✅ ADD THIS

                // ROLE BASED REDIRECT
                if (existingUser.Role == "Admin")
                    return RedirectToAction("Index", "Admin");

                else if (existingUser.Role == "Agent")
                    return RedirectToAction("Index", "Agent");

                else
                    return RedirectToAction("Index", "PrivateSeller");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View("~/Views/Home/Login.cshtml");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // remove all user data
            return RedirectToAction("Index", "Home"); // go to homepage
        }





    }
}