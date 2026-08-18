
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Data;
using RealEstatePortal.Models;
using System.Linq;

namespace RealEstatePortal.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ApplicationDbContext db;

        public AdminController(ApplicationDbContext context) : base(context)
        {
            db = context;
        }

        // DASHBOARD
        public IActionResult Index()
        {
            var listings = db.Listings.ToList();

            ViewBag.Total = listings.Count;
            ViewBag.Approved = listings.Count(x => x.Status == "Approved");
            ViewBag.Pending = listings.Count(x => x.Status == "Pending");
            ViewBag.Expired = listings.Count(x => x.Status == "Expired");

            ViewBag.Users = db.Users.Count();

            // For layout dropdown
            ViewBag.Messages = db.ContactMessages.ToList();
            ViewBag.UnreadCount = db.ContactMessages.Count(x => x.IsRead == false);

            // For dashboard card
            ViewBag.TotalMessages = db.ContactMessages.Count();

            return View();
        }

        public IActionResult Listings(string keyword, string category, string status, int? cityId)
        {
            var listings = db.Listings.AsQueryable();

            // KEYWORD FILTER
            if (!string.IsNullOrEmpty(keyword))
                listings = listings.Where(x => x.Title.ToLower().Contains(keyword.ToLower()));

            // CATEGORY FILTER
            if (!string.IsNullOrEmpty(category))
                listings = listings.Where(x => x.Category == category);

            // STATUS FILTER
            if (!string.IsNullOrEmpty(status))
                listings = listings.Where(x => x.Status == status);

            // CITY FILTER
            if (cityId.HasValue)
                listings = listings.Where(x => x.CityId == cityId);


            // ACTIVE CATEGORIES
            ViewBag.Categories = db.Categories
                .Where(x => x.IsActive)
                .ToList();

            ViewBag.Keyword = keyword;
            ViewBag.Category = category;
            ViewBag.Status = status;
            ViewBag.CityId = cityId;
            ViewBag.Cities = db.Cities.ToList();

            return View(listings.ToList());
        }
    
        public IActionResult ListingDetails(int id)
        {
            var listing = db.Listings
                            .Include(x => x.City)   
                            .Include(x => x.Images)   
                            .FirstOrDefault(x => x.Id == id);

            if (listing == null)
                return NotFound();

            var userName = db.Users
                             .Where(u => u.Id.ToString() == listing.UserId) 
                             .Select(u => u.Name)
                             .FirstOrDefault();

            ViewBag.UserName = userName;

            //  RETURN
            return View(listing);
        }

        public IActionResult Approve(int id)
        {
            var listing = db.Listings.Find(id);

            if (listing != null)
            {
                listing.Status = "Approved";
                db.SaveChanges();
            }

            return RedirectToAction("Listings");
        }

        public IActionResult Reject(int id)
        {
            var listing = db.Listings.Find(id);

            if (listing != null)
            {
                listing.Status = "Rejected";
                db.SaveChanges();
            }

            return RedirectToAction("Listings");
        }

        [HttpPost]
        public IActionResult DeleteListing(int id)
        {
            var listing = db.Listings.Find(id);

            if (listing != null)
            {
                db.Listings.Remove(listing);
                db.SaveChanges();
            }

            return RedirectToAction("Listings");
        }

    

        public IActionResult Users(string keyword, string email, string role, string status)
        {
            var users = db.Users
                          .Where(u => u.Role != "Admin")
                          .AsQueryable();

            // NAME SEARCH
            if (!string.IsNullOrEmpty(keyword))
            {
                users = users.Where(u =>
                    u.Name.ToLower().Contains(keyword.ToLower()));
            }

            // EMAIL SEARCH
            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(u =>
                    u.Email.ToLower().Contains(email.ToLower()));
            }

            // ROLE FILTER
            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role == role);
            }

            // STATUS FILTER
            if (!string.IsNullOrEmpty(status))
            {
                users = users.Where(u => u.Status == status);
            }

            // PASS BACK
            ViewBag.Keyword = keyword;
            ViewBag.Email = email;
            ViewBag.Role = role;
            ViewBag.Status = status;

            return View(users.ToList());
        }






        [HttpPost]
        public IActionResult DeleteUser(Guid id)
        {
            var user = db.Users.Find(id);

            if (user != null)
            {
                user.Status = "Blocked";  

                db.SaveChanges();
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult UnblockUser(Guid id)
        {
            var user = db.Users.Find(id);

            if (user != null)
            {
                user.Status = "Active";
                db.SaveChanges();
            }

            return RedirectToAction("Users");
        }

        // this all for category

        public IActionResult Categories()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (!string.IsNullOrEmpty(category.Name))
            {
                db.Categories.Add(category);
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }

        public IActionResult EditCategory(int id)
        {
            var category = db.Categories.Find(id);
            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            var data = db.Categories.Find(category.Id);

            if (data != null)
            {
                data.Name = category.Name;
                data.Description = category.Description;
                data.IsActive = category.IsActive;

                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }
        public IActionResult ActivateCategory(int id)
        {
            var category = db.Categories.Find(id);

            if (category != null)
            {
                category.IsActive = true;
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }

        public IActionResult DeactivateCategory(int id)
        {
            var category = db.Categories.Find(id);

            if (category != null)
            {
                category.IsActive = false;
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }
        //  LOCATIONS (CITY) 

        // Show all cities
        public IActionResult Locations()
        {
            var cities = db.Cities.ToList();
            return View(cities);
        }

        // Add city
        [HttpPost]
        public IActionResult AddCity(City city)
        {
            if (!string.IsNullOrEmpty(city.Name))
            {
                db.Cities.Add(city);
                db.SaveChanges();
            }

            return RedirectToAction("Locations");
        }

        // Delete city
        public IActionResult DeleteCity(int id)
        {
            var city = db.Cities.Find(id);

            if (city != null)
            {
                db.Cities.Remove(city);
                db.SaveChanges();
            }

            return RedirectToAction("Locations");
        }
        public IActionResult EditCity(int id)
        {
            var city = db.Cities.Find(id);

            if (city == null)
                return NotFound();

            return View(city);
        }

        [HttpPost]
        public IActionResult EditCity(City model)
        {
            var city = db.Cities.Find(model.Id);

            if (city == null)
                return NotFound();

            city.Name = model.Name;

            db.SaveChanges();


            return RedirectToAction("Locations");
        }

        public IActionResult DeleteCategory(int id)
        {
            var category = db.Categories.Find(id);

            if (category != null)
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }

            return RedirectToAction("Categories");
        }


        public IActionResult Messages()
        {
            var messages = db.ContactMessages
                .OrderByDescending(x => x.SentDate)
                .ToList();

            // Mark all as read
            foreach (var msg in messages.Where(x => !x.IsRead))
            {
                msg.IsRead = true;
            }
            db.SaveChanges();

            return View(messages);
        }


        public IActionResult Reports()
        {
            ViewBag.TotalUsers = db.Users.Count();

            ViewBag.TotalListings = db.Listings.Count();

            ViewBag.ApprovedListings =
                db.Listings.Count(x => x.Status == "Approved");

            ViewBag.PendingListings =
                db.Listings.Count(x => x.Status == "Pending");

            ViewBag.RejectedListings =
                db.Listings.Count(x => x.Status == "Rejected");

            ViewBag.ExpiredListings =
                db.Listings.Count(x => x.Status == "Expired");

            ViewBag.TotalCategories =
                db.Categories.Count();

            ViewBag.TotalCities =
                db.Cities.Count();

            ViewBag.TotalMessages =
                db.ContactMessages.Count();

            return View();
        }


        // Packages
        public IActionResult Packages()
        {
            var packages = db.Packages.ToList();
            return View(packages);
        }

        [HttpPost]
        public IActionResult AddPackage(Package package)
        {
            db.Packages.Add(package);
            db.SaveChanges();

            return RedirectToAction("Packages");
        }
        public IActionResult DeletePackage(int id)
        {
            var package = db.Packages.Find(id);

            if (package != null)
            {
                db.Packages.Remove(package);
                db.SaveChanges();
            }

            return RedirectToAction("Packages");
        }
        public IActionResult AssignPackage(Guid id)
        {
            var user = db.Users.Find(id);

            ViewBag.Packages = db.Packages
                .Where(x => x.IsActive)
                .ToList();

            return View(user);
        }

        [HttpPost]
        public IActionResult AssignPackage(Guid id, int packageId)
        {
            var user = db.Users.Find(id);

            var package = db.Packages.Find(packageId);

            if (user != null && package != null)
            {
                user.PackageId = package.Id;

                user.PackageExpiryDate =
                    DateTime.Now.AddDays(package.DurationDays);


                db.SaveChanges();
            }

            return RedirectToAction("Users");
        }


        public IActionResult EditPackage(int id)
        {
            var package = db.Packages.Find(id);

            return View(package);
        }

        [HttpPost]
        public IActionResult EditPackage(Package model)
        {
            var data = db.Packages.Find(model.Id);

            if (data != null)
            {
                data.Name = model.Name;
                data.Price = model.Price;
                data.ListingLimit = model.ListingLimit;
                data.DurationDays = model.DurationDays;
                data.IsActive = model.IsActive;

                db.SaveChanges();
            }

            return RedirectToAction("Packages");
        }


        // SETTINGS
        public IActionResult Settings()
        {
            var settings = db.Settings.FirstOrDefault();

            if (settings == null)
            {
                settings = new Settings
                {
                    CurrencySymbol = "PKR",
                    ListingsPerPage = 10,
                    FeaturedAdsEnabled = true
                };

                db.Settings.Add(settings);
                db.SaveChanges();
            }

            return View(settings);
        }

        [HttpPost]
        public IActionResult Settings(Settings model)
        {
            var settings = db.Settings.FirstOrDefault();

            if (settings != null)
            {
                settings.CurrencySymbol = model.CurrencySymbol;
                settings.ListingsPerPage = model.ListingsPerPage;
                settings.FeaturedAdsEnabled = model.FeaturedAdsEnabled;

                db.SaveChanges();

                TempData["success"] = "Settings updated successfully.";
            }

            return RedirectToAction("Settings");
        }

        public IActionResult PaypalSettings()
        {
            var setting = db.PaypalSettings.FirstOrDefault();

            if (setting == null)
            {
                setting = new PaypalSetting();
            }

            return View(setting);
        }

        [HttpPost]
        public IActionResult PaypalSettings(PaypalSetting model)
        {
            var setting = db.PaypalSettings.FirstOrDefault();

            if (setting == null)
            {
                db.PaypalSettings.Add(model);
            }
            else
            {
                setting.PaypalEmail = model.PaypalEmail;
                setting.ClientId = model.ClientId;
                setting.SandboxMode = model.SandboxMode;
            }

            db.SaveChanges();

            TempData["success"] = "PayPal settings updated successfully.";

            return RedirectToAction("PaypalSettings");
        }



    }
}


