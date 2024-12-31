namespace expensetracker.Models
{
    public class admin
    {
        
            public int TotalUsers { get; set; }
            public decimal TotalExpenses { get; set; }
            public int PendingApprovals { get; set; }
            public int RecentTransactions { get; set; }
            public List<string> RecentActivity { get; set; }
        

    }
}
