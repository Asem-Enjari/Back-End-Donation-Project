namespace SchoolDonation.API.DTOs
{
    // 1. For Donor (Creating the request)

    

    public class CreateSponsorshipDto
    {
        public string ItemType { get; set; } // "Stationery", "Tuition"
        public decimal Amount { get; set; }  // The cost
        public string Description { get; set; } // "50 Notebooks"
    }

    // 2. For Manager (Assigning a real student)
    public class AssignStudentDto
    {
        public int SponsorId { get; set; }
        public string StudentFullName { get; set; }

        // Change String to Int
        public int StudentGrade { get; set; } // e.g., 5, 9, 12

        public string Reason { get; set; }
    }


    // 3. For Viewing (Manager/Admin View)
    public class SponsorshipViewDto
    {
        public int SponsorId { get; set; }
        public string DonorName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string ItemType { get; set; }
        public DateTime Date { get; set; }

        // Status fields
        public string Status { get; set; }
        public bool IsApproved { get; set; }

        // Student fields (Might be null if not assigned yet)
        public string StudentFullName { get; set; }
        public int StudentGrade { get; set; }
    }
   
}