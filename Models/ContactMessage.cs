using System.ComponentModel.DataAnnotations;

namespace RealEstatePortal.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 3,
     ErrorMessage = "First name must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
     ErrorMessage = "Only letters allowed")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 3,
      ErrorMessage = "Last name must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$",
      ErrorMessage = "Only letters allowed")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
    }
}
