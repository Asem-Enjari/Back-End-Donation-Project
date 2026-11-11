using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public DateTime? JoinDate { get; set; }

    public string UserRole { get; set; } = null!;

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual Manager? Manager { get; set; }

    public virtual ICollection<StudentSponsorship> StudentSponsorships { get; set; } = new List<StudentSponsorship>();

    public virtual ICollection<UsedItem> UsedItems { get; set; } = new List<UsedItem>();

    public virtual ICollection<VolunteerActivity> VolunteerActivities { get; set; } = new List<VolunteerActivity>();
}
