using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int NeedId { get; set; }

    public string? Content { get; set; }

    public DateTime? PublishDate { get; set; }

    public virtual Need Need { get; set; } = null!;
}
