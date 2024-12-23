using expensetracker.DAL;
using expensetracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
namespace expensetracker.DAL
{
    public class expensedal
    {
        private readonly string connectionString;

        public expensedal(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void InsertUser(string name, int age, string address, string email, string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Create SQL Command
                    using (SqlCommand cmd = new SqlCommand("sp_insert", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@age", age);
                        cmd.Parameters.AddWithValue("@addr", address); // Ensure this is correctly added
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        // Execute the command
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Optional: Output success message
                        Console.WriteLine($"{rowsAffected} row(s) inserted successfully.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
       


        private readonly List<user> _users;

            public expensedal()
            {
                // Dummy data for demo purposes
                _users = new List<user>
            {
                new user { UserId = 1, Name = "John Doe", Email = "john@example.com", Password = "password123" },
                new user { UserId = 2, Name = "Jane Smith", Email = "jane@example.com", Password = "pass123" }
            };
            }

            public user AuthenticateUser(string email, string password)
            {
                return _users.FirstOrDefault(u => u.Email == email && u.Password == password);
            }
        }

    }


        //public IEnumerable<Category> GetCategories(int userId)
        //{
        //    using var connection = new SqlConnection(connectionString);
        //    connection.Open();
        //    var cmd = new SqlCommand("usp_GetCategories", connection)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    cmd.Parameters.AddWithValue("@UserId", userId);

        //    using var reader = cmd.ExecuteReader();
        //    var categories = new List<Category>();
        //    while (reader.Read())
        //    {
        //        categories.Add(new Category
        //        {
        //            Id = reader.GetInt32(0),
        //            Name = reader.GetString(1),
        //            UserId = userId,
        //            CreatedDate = reader.GetDateTime(2)
        //        });
        //    }
        //    return categories;
        //}

        //public void AddExpense(Expense expense)
        //{
        //    using var connection = new SqlConnection(connectionString);
        //    connection.Open();
        //    var cmd = new SqlCommand("usp_AddExpense", connection)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    cmd.Parameters.AddWithValue("@Amount", expense.Amount);
        //    cmd.Parameters.AddWithValue("@CategoryId", expense.CategoryId);
        //    cmd.Parameters.AddWithValue("@Date", expense.Date);
        //    cmd.Parameters.AddWithValue("@Description", expense.Description);
        //    cmd.Parameters.AddWithValue("@UserId", expense.UserId);
        //    cmd.ExecuteNonQuery();
        //}

        //public IEnumerable<Expense> GetExpenses(int userId)
        //{
        //    using var connection = new SqlConnection(connectionString);
        //    connection.Open();
        //    var cmd = new SqlCommand("usp_GetExpenses", connection)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    cmd.Parameters.AddWithValue("@UserId", userId);

        //    using var reader = cmd.ExecuteReader();
        //    var expenses = new List<Expense>();
        //    while (reader.Read())
        //    {
        //        expenses.Add(new Expense
        //        {
        //            Id = reader.GetInt32(0),
        //            Amount = reader.GetDecimal(1),
        //            CategoryName = reader.GetString(2),
        //            Date = reader.GetDateTime(3),
        //            Description = reader.GetString(4),
        //            CreatedDate = reader.GetDateTime(5),
        //            UserId = userId
        //        });
        //    }
        //    return expenses;
        //}
//    }
//}



