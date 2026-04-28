using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Data;
using RealEstatePortal.Models;

namespace RealEstatePortal.Controllers
{
    public class PrivateSellerController : BaseController
    {
        private readonly ApplicationDbContext db;

        public PrivateSellerController(ApplicationDbContext context)
        {
            db = context;
        }

        // ================= INDEX =================
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var listings = db.Listings
                .Where(x => x.UserId == userId)
                .ToList();

            ViewBag.Total = listings.Count;
            ViewBag.Active = listings.Count(x => x.Status == "Approved");
            ViewBag.Pending = listings.Count(x => x.Status == "Pending");
            ViewBag.Expired = listings.Count(x => x.Status == "Expired");

            return View(listings);
        }

        // ================= ADD PROPERTY =================
        public IActionResult AddProperty()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProperty(Listing model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            model.UserId = userId;
            model.Status = "Pending";
            model.CreatedDate = DateTime.Now;

            db.Listings.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        //[HttpPost]
        //public IActionResult AddProperty(Listing model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToList();

        //        return Content(string.Join(" || ", errors));
        //    }

        //    return Content("MODEL IS VALID");
        //}


        // ================= EDIT =================
        public IActionResult Edit(int id)
        {
            var listing = db.Listings.Find(id);

            if (listing == null)
                return NotFound();

            var userId = HttpContext.Session.GetString("UserId");

            if (listing.UserId != userId)
                return Unauthorized();

            return View(listing);
        }

        [HttpPost]
        public IActionResult Edit(Listing model)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

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

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================= DELETE =================
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var listing = db.Listings.FirstOrDefault(x => x.Id == id);

            if (listing == null)
                return NotFound();

            if (listing.UserId != userId)
                return Unauthorized();

            db.Listings.Remove(listing);
            db.SaveChanges();

            return RedirectToAction("Index");
        }








    }
}