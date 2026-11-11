using System;
using System.Collections.Generic;

namespace SchoolDonation.API.Models;

public partial class Manager
{
    public int ManagerId { get; set; }

    public string EmployeeNumber { get; set; } = null!;

    public virtual User ManagerNavigation { get; set; } = null!;

    public virtual School? School { get; set; }
}
