using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class School
{
    public int SchoolId { get; set; }

    public int ManagerId { get; set; }

    public string Name { get; set; } = null!;

    public int? Capacity { get; set; }

    public string? Status { get; set; }

    public string Governorate { get; set; } = null!;

    public string City { get; set; } = null!;

    public string? Street { get; set; }

    public virtual Manager Manager { get; set; } = null!;

    public virtual ICollection<Need> Needs { get; set; } = new List<Need>();

    public virtual SchoolLocation? SchoolLocation { get; set; }

    public virtual ICollection<StudentSponsorship> StudentSponsorships { get; set; } = new List<StudentSponsorship>();

    public virtual ICollection<UsedItem> UsedItems { get; set; } = new List<UsedItem>();

    public virtual ICollection<VolunteerActivity> VolunteerActivities { get; set; } = new List<VolunteerActivity>();
}
