using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class SchoolLocation
{
    public int LocationId { get; set; }

    public int SchoolId { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public virtual School School { get; set; } = null!;
}
