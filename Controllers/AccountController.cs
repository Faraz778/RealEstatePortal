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
        [HttpPost]
        public IActionResult Register(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // ROLE-BASED DEFAULT IMAGE
            model.ProfileImage = model.Role switch
            {
                "Admin" => "/images/defaults/admin.png",
                "Agent" => "/images/defaults/agent.png",
                "PrivateSeller" => "/images/defaults/seller.png",
                _ => "/images/defaults/seller.png"
            };

            _context.Users.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }


        // GET: Login Page 
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
                // BLOCK CHECK
                if (existingUser.Status == "Blocked")
                {
                    ViewBag.Error = "Your account is blocked. Contact admin.";
                    return View("~/Views/Home/Login.cshtml");
                }

                // SESSION VALUES
                HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
                HttpContext.Session.SetString("UserEmail", existingUser.Email);
                HttpContext.Session.SetString("UserRole", existingUser.Role);
                HttpContext.Session.SetString("UserName", existingUser.Name);

                // ROLE-BASED DEFAULT IMAGE 
                string defaultImage = existingUser.Role switch
                {
                    "Admin" => "/images/defaults/admin.png",
                    "Agent" => "/images/defaults/agent.png",
                    "PrivateSeller" => "/images/defaults/seller.png",
                    _ => "/images/defaults/seller.png"
                };

                // FINAL IMAGE (DB IMAGE OR DEFAULT)
                HttpContext.Session.SetString(
                    "UserImage",
                    existingUser.ProfileImage ?? defaultImage
                );

                // ROLE REDIRECT
                if (existingUser.Role == "Admin")
                    return RedirectToAction("Index", "Admin");

                else if (existingUser.Role == "Agent")
                    return RedirectToAction("Index", "Agent");

                else if (existingUser.Role == "PrivateSeller")
                    return RedirectToAction("Index", "PrivateSeller");

                else
                    return RedirectToAction("AccessDenied", "Home");
            }

            ViewBag.Error = "Invalid Email or Password";
            return View("~/Views/Home/Login.cshtml");
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // remove all user data
            return RedirectToAction("Index", "Home"); // go to homepage
        }


        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Id.ToString() == userId);

            return View(user);
        }


        public IActionResult EditProfile()
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (userIdString == null)
            {
                return RedirectToAction("Login");
            }

            var userId = Guid.Parse(userIdString);

            var user = _context.Users.Find(userId);

            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User model, string Password, IFormFile imageFile)
        {
            var userIdString = HttpContext.Session.GetString("UserId");

            if (userIdString == null)
            {
                return RedirectToAction("Login");
            }

            var userId = Guid.Parse(userIdString);
            var user = _context.Users.Find(userId);

            if (user != null)
            {
                user.Name = model.Name;
                user.Email = model.Email;


                user.AgencyName = model.AgencyName;

                user.OfficeAddress = model.OfficeAddress;

                user.WhatsApp = model.WhatsApp;

                user.AboutAgent = model.AboutAgent;

                user.ExperienceYears = model.ExperienceYears;

                user.CompanyLogo = model.CompanyLogo;



                if (imageFile != null)
                {
                    string folder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images/users"
                    );

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string fileName = Guid.NewGuid() +
                                      Path.GetExtension(imageFile.FileName);

                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    user.ProfileImage = "/images/users/" + fileName;
                }



                // Update password ONLY if entered
                if (!string.IsNullOrEmpty(Password))
                {
                    user.Password = Password; //  hash later
                }

                _context.SaveChanges();

                // Update Session 
                HttpContext.Session.SetString("UserName", user.Name ?? "");

                HttpContext.Session.SetString(
                "UserImage",
                user.ProfileImage ?? "/images/defaults/seller.png"
             );

                TempData["success"] = "Profile updated successfully!";
            }

            return RedirectToAction("Profile");
        }


    }
}








