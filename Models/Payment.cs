using System.ComponentModel.DataAnnotations;

namespace SchoolDonation.API.Models
{
	public partial class Payment
	{
		public int PaymentId { get; set; } 

		public int NeedId { get; set; }

		public int SchoolId { get; set; }

		public decimal Amount { get; set; }

		public DateTime PaymentDate { get; set; }

		public string? TransactionReference { get; set; }

		public virtual Need Need { get; set; } = null!;
		public virtual School School { get; set; } = null!;
	}

}
