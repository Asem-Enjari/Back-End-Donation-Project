using System.Diagnostics.CodeAnalysis;

namespace SchoolDonation.API.DTOs
{
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } 
        public string Email { get; set; } 
        public string Role { get; set; } 
    }
}