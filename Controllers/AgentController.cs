
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Data;
using RealEstatePortal.Models;

namespace RealEstatePortal.Controllers
{
    public class AgentController : BaseController
    {
        private readonly ApplicationDbContext db;

        public AgentController(ApplicationDbContext context) : base(context)
        {
            db = context;
        }

        private string GetUserId()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                throw new Exception("User not logged in");

            return userId;
        }

        
        public IActionResult Index()
        {
            try
            {
                var userId = GetUserId();

                var listings = db.Listings
                    .Where(x => x.UserId == userId)
                    .ToList();

                ViewBag.TotalListings = listings.Count;
                ViewBag.PendingListings = listings.Count(x => x.Status == "Pending");
                ViewBag.ApprovedListings = listings.Count(x => x.Status == "Approved");
                ViewBag.ExpiredListings = listings.Count(x => x.Status == "Expired");

                return View();
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        //ADD PROPERTY(GET)
        public IActionResult AddProperty()
        {
            ViewBag.Cities = db.Cities.ToList();



            ViewBag.Categories = db.Categories
                .Where(x => x.IsActive)
                .ToList();

            return View();
        }



        [HttpPost]
        public IActionResult AddProperty(Listing model)
        {
            try
            {
                var userId = GetUserId();

                model.UserId = userId;
                model.Status = "Pending";
                model.CreatedDate = DateTime.Now;

                var user = db.Users
                    .Include(x => x.Package)
                    .FirstOrDefault(x => x.Id.ToString() == userId);

                // NO PACKAGE
                if (user.Package == null)
                {
                    TempData["error"] = "No package assigned.";
                    return RedirectToAction("Index");
                }

                // PACKAGE EXPIRED
                if (DateTime.Now > user.PackageExpiryDate)
                {
                    TempData["error"] = "Package expired.";
                    return RedirectToAction("Index");
                }

                // COUNT CURRENT LISTINGS
                var totalListings = db.Listings
                    .Count(x => x.UserId == userId);

                // CHECK LIMIT
                if (totalListings >= user.Package.ListingLimit)
                {
                    TempData["error"] = "Listing limit reached.";
                    return RedirectToAction("Index");
                }

                // ALLOWED IMAGE EXTENSIONS
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                // VALIDATE IMAGES
                if (model.ImageFiles != null && model.ImageFiles.Count > 0)
                {
                    foreach (var file in model.ImageFiles)
                    {
                        var extension = Path.GetExtension(file.FileName).ToLower();

                        // CHECK FILE EXTENSION
                        if (!allowedExtensions.Contains(extension))
                        {
                            TempData["error"] = "Only JPG, JPEG and PNG files are allowed.";

                            ViewBag.Cities = db.Cities.ToList();

                            ViewBag.Categories = db.Categories
                                .Where(x => x.IsActive)
                                .ToList();

                            return View(model);
                        }

                        
                        if (!file.ContentType.StartsWith("image/"))
                        {
                            TempData["error"] = "Invalid image file.";

                            ViewBag.Cities = db.Cities.ToList();

                            ViewBag.Categories = db.Categories
                                .Where(x => x.IsActive)
                                .ToList();

                            return View(model);
                        }
                    }
                }

                // SAVE LISTING
                db.Listings.Add(model);
                db.SaveChanges();

                // MULTIPLE IMAGE UPLOAD
                if (model.ImageFiles != null && model.ImageFiles.Count > 0)
                {
                    string folder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/images"
                    );

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    foreach (var file in model.ImageFiles)
                    {
                        string fileName = Guid.NewGuid() +
                                          Path.GetExtension(file.FileName);

                        string filePath = Path.Combine(folder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        db.ListingImages.Add(new ListingImage
                        {
                            ListingId = model.Id,
                            ImagePath = "/images/" + fileName
                        });
                    }

                    db.SaveChanges();
                }

                TempData["success"] = "Property added successfully.";

                return RedirectToAction("AllListings");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }



        public IActionResult AllListings()
        {
            try
            {
                var userId = GetUserId();

                var listings = db.Listings
                    .Include(x => x.Images)
                    .Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToList();

                return View(listings);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }


        // PENDING
        public IActionResult PendingListings()
        {
            var userId = GetUserId();

            var listings = db.Listings
                        .Include(x => x.Images) 
                .Where(x => x.UserId == userId && x.Status == "Pending")
                .ToList();

            return View(listings);
        }

        // APPROVED
        public IActionResult ApprovedListings()
        {
            var userId = GetUserId();

            var listings = db.Listings
                        .Include(x => x.Images)  
                .Where(x => x.UserId == userId && x.Status == "Approved")
                .ToList();

            return View(listings);
        }


        public IActionResult RejectedListings()
        {
            var userId = GetUserId();

            var listings = db.Listings
                .Include(x => x.Images)
                .Where(x => x.UserId == userId && x.Status == "Rejected")
                .ToList();

            return View(listings);
        }

        public IActionResult Edit(int id)
        {
            //  images
            var listing = db.Listings
                .Include(x => x.Images) 
                .FirstOrDefault(x => x.Id == id);

            if (listing == null)
                return NotFound();

            var userId = GetUserId();

            if (listing.UserId != userId)
                return Unauthorized();

            ViewBag.Cities = db.Cities.ToList();

            ViewBag.Categories = db.Categories
                            .Where(x => x.IsActive)
                            .ToList();

            return View(listing);
        }

        [HttpPost]
        public IActionResult Edit(Listing model)
        {
            var userId = GetUserId();

            var listing = db.Listings.FirstOrDefault(x => x.Id == model.Id);

            if (listing == null)
                return NotFound();

            if (listing.UserId != userId)
                return Unauthorized();

            listing.Title = model.Title;
            listing.Price = model.Price;
            listing.Description = model.Description;
            listing.Category = model.Category;
            listing.Location = model.Location;
            listing.CityId = model.CityId;
            listing.HideEmail = model.HideEmail;


            //  MULTIPLE IMAGE UPLOAD
            if (model.ImageFiles != null && model.ImageFiles.Count > 0)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                foreach (var file in model.ImageFiles)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    db.ListingImages.Add(new ListingImage
                    {
                        ListingId = listing.Id,
                        ImagePath = "/images/" + fileName
                    });
                }
            }

            db.SaveChanges();

            return RedirectToAction("AllListings");
        }


        public IActionResult Delete(int id)
        {
            //  images
            var listing = db.Listings
                .Include(x => x.Images)  
                .FirstOrDefault(x => x.Id == id);

            if (listing == null)
                return NotFound();

            var userId = GetUserId();

            if (listing.UserId != userId)
                return Unauthorized();

            //  Delete image files from folder
            foreach (var img in listing.Images)
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.ImagePath.TrimStart('/'));

                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
            }

            db.Listings.Remove(listing);
            db.SaveChanges();

            return RedirectToAction("AllListings");
        }
        public IActionResult DeleteImage(int id)
        {
            var userId = GetUserId();

            var image = db.ListingImages
                .Include(x => x.Listing)
                .FirstOrDefault(x => x.Id == id);

            if (image == null)
                return NotFound();

            if (image.Listing.UserId != userId)
                return Unauthorized();

            // Delete file from folder
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImagePath.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);

            db.ListingImages.Remove(image);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = image.ListingId });
        }

        public IActionResult Packages()
        {
            var packages = db.Packages
                .Where(x => x.IsActive)
                .ToList();

            return View(packages);
        }

        public IActionResult BuyPackage(int id)
        {
            var package = db.Packages.Find(id);

            if (package == null)
                return NotFound();

            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            // SAVE PAYMENT
            var payment = new Payment
            {
                UserId = userId,
                PackageId = package.Id,
                Amount = package.Price,
                PaymentMethod = "Cash",
                Status = "Paid",
                PaymentDate = DateTime.Now
            };

            db.Payments.Add(payment);

            // ASSIGN PACKAGE
            var user = db.Users.Find(Guid.Parse(userId));

            user.PackageId = package.Id;

            user.PackageExpiryDate =
                DateTime.Now.AddDays(package.DurationDays);

            db.SaveChanges();

            TempData["success"] =
                "Package purchased successfully.";

            return RedirectToAction("Packages");
        }






    }
}


