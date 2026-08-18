using RealEstatePortal.Models;

public class Payment
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int PackageId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; }

    public string Status { get; set; }

    public DateTime PaymentDate { get; set; }

    public virtual Package Package { get; set; }
}