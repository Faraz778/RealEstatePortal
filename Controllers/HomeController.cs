        
using Microsoft.AspNetCore.Mvc;
using RealEstatePortal.Data;
using RealEstatePortal.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ApplicationDbContext db;

    public HomeController(ApplicationDbContext context)
    {
        db = context;
    }


 
    // ===== INDEX =====
    public IActionResult Index()
    {
        // Load cities
        ViewBag.Cities = db.Cities.ToList();

        // LOAD ONLY ACTIVE CATEGORIES
        ViewBag.Categories = db.Categories
            .Where(x => x.IsActive)
            .ToList();

     


        var settings = db.Settings.FirstOrDefault();

        var listings = new List<Listing>();

        if (settings != null && settings.FeaturedAdsEnabled)
        {
            listings = db.Listings
                .Include(x => x.Images)
                .Include(x => x.City)
                .OrderByDescending(l => l.CreatedDate)
                .Take(settings.ListingsPerPage)
                .ToList();
        }


        // Stats
        ViewBag.TotalUsers = db.Users.Count();
        ViewBag.TotalListings = db.Listings.Count();
        ViewBag.ApprovedListings = db.Listings.Count(x => x.Status == "Approved");
        ViewBag.TotalCities = db.Cities.Count();

        return View(listings);
    }

    //  STATIC PAGES 

    public IActionResult Contact()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(ContactMessage model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.SentDate = DateTime.Now;
        model.IsRead = false;

        db.ContactMessages.Add(model);
        db.SaveChanges();

        TempData["Success"] = "Message sent successfully!";
        return RedirectToAction("Contact");
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult FAQ()
    {
        return View();
    }

    public IActionResult News()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult MortgageCalculator()
    {
        return View();
    }



    public IActionResult Search(string keyword, int? cityId, string category, string purpose, string sellerType)
    {
        var listings = db.Listings
            .Include(x => x.Images)
            .Include(x => x.City)
            .Where(l => l.Status == "Approved")
            .AsQueryable();

        // KEYWORD FILTER
        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = keyword.ToLower();
            listings = listings.Where(l =>
                l.Title.ToLower().Contains(keyword) ||
                l.Description.ToLower().Contains(keyword));
        }

        // CITY FILTER
        if (cityId.HasValue)
            listings = listings.Where(l => l.CityId == cityId);

        // CATEGORY FILTER
        if (!string.IsNullOrEmpty(category))
            listings = listings.Where(l => l.Category == category);

        // PURPOSE FILTER
        if (!string.IsNullOrEmpty(purpose))
            listings = listings.Where(l => l.Purpose == purpose);

        // SELLER TYPE FILTER 
        if (!string.IsNullOrEmpty(sellerType))
        {
            var userIds = db.Users
                .Where(u => u.Role == sellerType)
                .Select(u => u.Id.ToString())
                .ToList();

            listings = listings.Where(l => userIds.Contains(l.UserId));
        }

        // PASS TO VIEW
        ViewBag.Keyword = keyword;
        ViewBag.CityId = cityId;
        ViewBag.Category = category;
        ViewBag.Purpose = purpose;
        ViewBag.SellerType = sellerType;

        ViewBag.Cities = db.Cities.ToList();

        ViewBag.Categories = db.Categories
            .Where(x => x.IsActive)
            .ToList();

        return View(listings.ToList());
    }

    public IActionResult Details(int id)
    {
        var listing = db.Listings
            .Include(x => x.Images)
            .Include(x => x.City)
            .FirstOrDefault(x => x.Id == id);

        if (listing == null)
            return NotFound();

        var owner = db.Users
            .FirstOrDefault(x => x.Id.ToString() == listing.UserId);

        ViewBag.OwnerEmail = owner?.Email;


        return View(listing);
    }

    // Errorr 
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}



