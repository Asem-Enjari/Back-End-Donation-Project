using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class Donation
{
    public int DonationId { get; set; }

    public int DonorId { get; set; }

    public int NeedId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? DonationDate { get; set; }

    public string? TransactionReference { get; set; }

    public virtual User Donor { get; set; } = null!;

    public virtual Need Need { get; set; } = null!;
}
