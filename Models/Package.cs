using System.ComponentModel.DataAnnotations;

namespace RealEstatePortal.Models
{
    public class Package
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int ListingLimit { get; set; }

        public int DurationDays { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
}