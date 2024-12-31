namespace expensetracker.Models
{
    public class ExpenseModel
    {
        public int Expenseid { get; set; }
        public int ExpenseTypeID { get; set; }
        public int Amount { get; set; }
        public DateTime DateofExpense { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Createdat { get; set; }
        public string Billcopy { get; set; }
        public string Status { get; set; }
        public int ApprovedAmount { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}
