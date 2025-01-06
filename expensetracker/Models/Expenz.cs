using System.ComponentModel.DataAnnotations;

namespace expensetracker.Models
{

    public class Expenz
    {
        public int Expenseid { get; set; }

        [Required(ErrorMessage = "Expense Type is required.")]
        public int ExpenseTypeID { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "Date of Expense is required.")]
        public DateTime DateofExpense { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(150, ErrorMessage = "Description cannot exceed 150 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Created By is required.")]
        public string CreatedBy { get; set; }

        [Required(ErrorMessage = "Created At date is required.")]
        public DateTime CreatedAt { get; set; }

        public string? BillCopy { get; set; } // Now holds the Base64 string

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        public string ExpenseTypeName { get; set; } // Optional
        public int ApprovedAmount { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
    public class Approved
    {
        public int ExpenseId { get; set; }
        public int ApprovedAmount { get; set; }
        public DateTime Approveddate
        {
            get; set;
        }
    }
}
