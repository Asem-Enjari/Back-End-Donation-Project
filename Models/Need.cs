using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class Need
{
	public int NeedId { get; set; }

	public int SchoolId { get; set; }

	public string Title { get; set; } = null!;

	public string? Description { get; set; }

	public decimal Cost { get; set; }

	public decimal? CollectedAmount { get; set; }

	public string? Status { get; set; }

	public DateTime? PublishDate { get; set; }

	public bool IsApproved { get; set; }

	public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

	public virtual ICollection<Image> Images { get; set; } = new List<Image>();

	public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

	public virtual School School { get; set; } = null!;
}