namespace SchoolDonation.API.DTOs
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        
        // "Manager", "Donor", "Volunteer"
        public string UserRole { get; set; } 
    }
}