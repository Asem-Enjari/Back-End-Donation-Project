namespace SchoolDonation.API.DTOs
{
    public enum EducationLevel
    {
        Basic = 1,       // Grades 1-6
        Preparatory = 2, // Grades 7-9
        Secondary = 3    // Grades 10-12
    }
    public class PublicSponsorshipDto
    {
        public int SponsorId { get; set; }
        public string SchoolName { get; set; }

        // Generic info (e.g. "Grade 5")
        public int StudentGrade { get; set; }

        // e.g. "Basic Education"
        public string EducationLevel { get; set; }

        public string ItemType { get; set; } // "Stationery"
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}