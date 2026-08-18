namespace RealEstatePortal.Models
{
    public class PaypalSetting
    {
        public int Id { get; set; }

        public string? PaypalEmail { get; set; }

        public string? ClientId { get; set; }

        public bool SandboxMode { get; set; }
    }
}