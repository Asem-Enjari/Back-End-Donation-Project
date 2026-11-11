namespace SchoolDonation.API.DTOs
{
    public class MapPinDto
    {
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        
        // "Operational", "Damaged", "Under Renovation"
        // Frontend will use this to pick the pin color (Green, Red, Yellow)
        public string Status { get; set; } 
    }
}