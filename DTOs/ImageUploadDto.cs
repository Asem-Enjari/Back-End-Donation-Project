using Microsoft.AspNetCore.Http; // Required for IFormFile

namespace SchoolDonation.API.DTOs
{
    public class ImageUploadDto
    {
        public int NeedId { get; set; }
        
        // "Before", "During", "After", "Invoice"
        public string ImageType { get; set; } 
        
        public IFormFile File { get; set; }
    }
}