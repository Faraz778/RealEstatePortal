namespace RealEstatePortal.Models
{
    public class User
    {
       
            public Guid Id { get; set; } = Guid.NewGuid();

            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }

            public string Role { get; set; } // PrivateSeller, Agent, Admin
        
    }
}
