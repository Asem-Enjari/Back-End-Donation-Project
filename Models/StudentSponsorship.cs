using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class StudentSponsorship
{
    public int SponsorId { get; set; }

    public int? DonorId { get; set; }

    public int SchoolId { get; set; }

    public string ItemType { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? SponsorshipDate { get; set; }

    public bool? IsAnonymous { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public bool IsApproved { get; set; }

    public string? StudentFullName { get; set; }

    public int? StudentGrade { get; set; }

    public string? SponsorshipReason { get; set; }

    public string? AdminFeedback { get; set; }

    public virtual User Donor { get; set; } = null!;

    public virtual School School { get; set; } = null!;
}
