using SchoolDonation.API.Models;
using BCrypt.Net;

namespace SchoolDonation.API.Data
{
    public static class DbInitializer
    {
        public static void Seed(SchoolDonationContext context)
        {
            // Check if DB has any users
           if (context.Users.Any()) 
                return; // DB has been seeded

            // Create the Super Admin
            var adminUser = new User
            {
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@edu.gov.sy", // The Login Email
                PhoneNumber = "0000000000",
                UserRole = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!") // The Login Password
            };

            context.Users.Add(adminUser);
            context.SaveChanges();

        }
    }
}