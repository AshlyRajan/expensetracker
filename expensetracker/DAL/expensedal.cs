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

        public bool AddUser(property user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("[sp_insert]", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Age", user.Age);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Dare", user.Date);
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    // Log the exception (if logging is configured)
                    throw new Exception("Error adding user: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // Method to Check if Email Already Exists
        public bool IsEmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_IsEmailExists", connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Email", email);

                    connection.Open();
                    var result = cmd.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking email existence: " + ex.Message);
                }
                finally
                {
                    connection.Close();
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



