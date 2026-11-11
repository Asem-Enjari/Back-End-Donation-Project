namespace SchoolDonation.API.DTOs
{
    // For the Top Cards
    public class HomeStatsDto
    {
        public int TotalSchools { get; set; }
        public decimal TotalCollectedAmount { get; set; } // The $3500
        public int ActiveProjects { get; set; }
    }

    // For the School Cards (The List)
    public class SchoolCardDto
    {
        public int SchoolId { get; set; }
        public string Name { get; set; }      // e.g. "Al-Bayan Basic School"
        public string Status { get; set; }    // e.g. "Damaged"
        public string City { get; set; }      // e.g. "Aleppo"
        public int Capacity { get; set; }
        public int NeedsCount { get; set; }   // "2 Items"
        public string ImageUrl { get; set; }

        // This is for the Green Progress Bar
        public int ProgressPercentage { get; set; }
    }
     public class RecentDonationDto
    {
        public string DonorName { get; set; }
        public decimal Amount { get; set; }
        public string TimeAgo { get; set; } // e.g. "2 mins ago", "1 hour ago"
    }

    // For the Search Bar Result
    // We can reuse SchoolCardDto, but maybe we want a simpler list for the dropdown
    public class SearchResultDto
    {
        public int Id { get; set; }
        public string Type { get; set; } // "School" or "Project"
        public string Title { get; set; }
        public string Subtitle { get; set; } // City for School, SchoolName for Project
    }
}