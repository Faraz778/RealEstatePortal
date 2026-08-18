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


        public string? Purpose { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? UserId { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }   


        public bool HideEmail { get; set; }

        // Image path stored in DB
        public string? ImagePath { get; set; }


        public ICollection<ListingImage> Images { get; set; }
        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }



        // not stored in DB (used for upload)
        [NotMapped]
        public IFormFile? ImageFile { get; set; }



    }
}
