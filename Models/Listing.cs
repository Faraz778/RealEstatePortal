using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstatePortal.Models
{
    public class Listing
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public string? Location { get; set; }

        public string? Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? UserId { get; set; }
    }
}
