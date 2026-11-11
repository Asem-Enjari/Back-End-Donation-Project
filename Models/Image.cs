using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public int NeedId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string ImageType { get; set; } = null!;

    public DateTime? UploadDate { get; set; }

    public virtual Need Need { get; set; } = null!;
}
