namespace RealEstatePortal.Models
{
    public class Settings
    {
        public int Id { get; set; }

        public string CurrencySymbol { get; set; }

        public int ListingsPerPage { get; set; }

        public bool FeaturedAdsEnabled { get; set; }
    }
}