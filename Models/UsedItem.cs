using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class UsedItem
{
    public int ItemId { get; set; }

    public int DonorId { get; set; }

    public int? SchoolId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? DonationDate { get; set; }

    public string? Status { get; set; }

    public string? ImageUrl { get; set; }

    public virtual User Donor { get; set; } = null!;

    public virtual School? School { get; set; }
}
