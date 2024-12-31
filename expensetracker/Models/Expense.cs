using System.ComponentModel.DataAnnotations;

namespace expensetracker.Models
{
    public class Expense
    {
        public int ExpenseID { get; set; } // Primary Key for Expenses

        [Required(ErrorMessage = "Expense Type is required")]
        public string ExpenseType { get; set; } // Name of the Expense Type

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; } // Amount of the Expense

        [Required(ErrorMessage = "Date of Expense is required")]
        public DateTime DateOfExpense { get; set; } // Date when the Expense occurred

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } // Description or notes about the Expense

        [Required(ErrorMessage = "Created By is required")]
        public string CreatedBy { get; set; } // User who created the Expense

        public DateTime CreatedAt { get; set; } = DateTime.Now; // Timestamp of when the Expense was created

        [Required(ErrorMessage = "Expense Type ID is required")]
        public int ExpenseTypeID { get; set; } // Foreign Key for Expense Type

        public string BillCopyPath {  get; set; } // Path to the uploaded bill copy


    }

    public class ExpenseTypeModel
    {
        public int ExpenseTypeID { get; set; }
        public string ExpenseType { get; set; } // Changed the property name from 'ExpenseType' to 'Name'
    }
}

