using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class VolunteerActivity
{
    public int ActivityId { get; set; }

    public int? VolunteerId { get; set; }

    public int? SchoolId { get; set; }

    public string ActivityType { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? ActivityDate { get; set; }

    public string? Status { get; set; }

    public virtual School? School { get; set; }

    public virtual User? Volunteer { get; set; }
}
