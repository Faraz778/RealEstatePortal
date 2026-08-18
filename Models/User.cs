using System.ComponentModel.DataAnnotations;


namespace RealEstatePortal.Models
{
    public class User
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Password { get; set; }

        public string Role { get; set; } // PrivateSeller, Agent, Admin
        public string Status { get; set; } = "Active";

        public string? ProfileImage { get; set; }



        // AGENT PROFILE FIELDS

        public string? AgencyName { get; set; }

        public string? OfficeAddress { get; set; }

        public string? WhatsApp { get; set; }

        public string? AboutAgent { get; set; }

        public int? ExperienceYears { get; set; }

        public string? CompanyLogo { get; set; }


        //PACKAGES

        public int? PackageId { get; set; }

        public DateTime? PackageExpiryDate { get; set; }


        public Package? Package { get; set; }
    }
}
