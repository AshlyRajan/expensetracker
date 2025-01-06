using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using expensetracker.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace expensetracker.DAL
{
    public class UserDAL
    {
        private readonly string connectionString;

        public UserDAL(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task InsertExpenzAsync(int expenseTypeID, int amount, DateTime dateOfExpense, string description, string createdBy, DateTime createdAt, string billCopy, string status)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("InsertExpenz", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@ExpenseTypeID", expenseTypeID);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@DateofExpense", dateOfExpense);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@CreatedBy", createdBy);
                    command.Parameters.AddWithValue("@CreatedAt", createdAt);
                    command.Parameters.AddWithValue("@BillCopy", string.IsNullOrEmpty(billCopy) ? (object)DBNull.Value : billCopy); // Store Base64 string
                    command.Parameters.AddWithValue("@Status", status);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }


        // Get all Expense Types from the ExpenseTypes table
        public List<SelectListItem> GetExpenseTypes()
        {
            List<SelectListItem> expenseTypes = new List<SelectListItem>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT ExpenseTypeID, ExpenseType FROM ExpenseTypes";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        expenseTypes.Add(new SelectListItem
                        {
                            Value = reader["ExpenseTypeID"].ToString(),
                            Text = reader["ExpenseType"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error fetching expense types: {ex.Message}");
            }

            return expenseTypes;
        }
        public List<Expenz> GetExpensesByUser(string username)
        {
            List<Expenz> expenses = new List<Expenz>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetExpensesWithApprovedAmountByUser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CreatedBy", username);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        expenses.Add(new Expenz
                        {
                            Expenseid = Convert.ToInt32(reader["Expenseid"]),
                            ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                            Amount = Convert.ToInt32(reader["Amount"]),
                            DateofExpense = Convert.ToDateTime(reader["DateofExpense"]),
                            Description = reader["Description"].ToString(),
                            CreatedBy = reader["CreatedBy"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            BillCopy = reader["BillCopy"].ToString(),
                            Status = reader["Status"].ToString(),
                            ApprovedAmount = Convert.ToInt32(reader["ApprovedAmount"]),
                            ApprovedDate = Convert.ToDateTime(reader["ApprovedDate"])
                        });
                    }
                }
            }

            return expenses;
        }





        public void UpdateExpenz(int expenseid, int expenseTypeID, decimal amount, DateTime dateofExpense, string description, string billCopy, string status)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("UpdateExpenz", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Expenseid", expenseid);
                    command.Parameters.AddWithValue("@ExpenseTypeID", expenseTypeID);
                    command.Parameters.AddWithValue("@Amount", amount);
                    command.Parameters.AddWithValue("@DateofExpense", dateofExpense);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@BillCopy", billCopy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Status", status);

                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Expense with ID {expenseid} updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating expense with ID {expenseid}: {ex.Message}");
                        throw;
                    }
                }
            }
        }


        // Delete expense method
        public void DeleteExpenz(int expenseid)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DeleteExpenzbyuser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Expenseid", expenseid);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error deleting expense with ID {expenseid}: {ex.Message}");
                    }
                }
            }
        }




        public async Task<List<Expenz>> GetUserExpensesAsync(string createdBy)
        {
            List<Expenz> expenses = new List<Expenz>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("GetUserExpenses", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CreatedBy", createdBy);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            expenses.Add(new Expenz
                            {
                                Expenseid = reader.GetInt32(reader.GetOrdinal("ExpenseID")),
                                ExpenseTypeID = reader.GetInt32(reader.GetOrdinal("ExpenseTypeID")),
                                Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
                                DateofExpense = reader.GetDateTime(reader.GetOrdinal("DateofExpense")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                BillCopy = reader.IsDBNull(reader.GetOrdinal("BillCopy")) ? null : reader.GetString(reader.GetOrdinal("BillCopy"))
                            });
                        }
                    }
                }
            }

            return expenses;
        }
        public IEnumerable<Expenz> GetExpensesByUserby(string username)
        {
            var expenses = new List<Expenz>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("GetExpensesBylog", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CreatedBy", username);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var expense = new Expenz
                            {
                                Expenseid = reader.GetInt32(reader.GetOrdinal("Expenseid")),
                                ExpenseTypeID = reader.GetInt32(reader.GetOrdinal("ExpenseTypeID")),
                                Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
                                DateofExpense = reader.GetDateTime(reader.GetOrdinal("DateofExpense")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("Createdat")),
                                BillCopy = reader.GetString(reader.GetOrdinal("Billcopy")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            };
                            expenses.Add(expense);
                        }
                    }
                }
            }
            return expenses;
        }

        // Method to delete an expense by its ID
        public void DeleteExpenzby(int expenseid)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DeleteExpenz", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Expenseid", expenseid);

                    command.ExecuteNonQuery();
                }
            }
        }
  




    public List<Expenz> GetAllExpenses()
                    {
                        List<Expenz> expenses = new List<Expenz>();

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string query = @"
            SELECT 
                e.Expenseid, 
                e.ExpenseTypeID, 
                et.ExpenseType,   -- Ensure this is correctly aliased as 'et.ExpenseType'
                e.Amount, 
                e.DateofExpense, 
                e.Description, 
                e.CreatedBy, 
                e.CreatedAt, 
                e.BillCopy, 
                e.Status
            FROM 
                Expenz e
            JOIN 
                ExpenseTypes et ON e.ExpenseTypeID = et.ExpenseTypeID";

                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();

                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                expenses.Add(new Expenz
                                {
                                    Expenseid = Convert.ToInt32(reader["Expenseid"]),
                                    ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                                    ExpenseTypeName = reader["ExpenseType"]?.ToString(),  // Make sure it's not null
                                    Amount = Convert.ToInt32(reader["Amount"]),
                                    DateofExpense = Convert.ToDateTime(reader["DateofExpense"]),
                                    Description = reader["Description"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                    BillCopy = reader["BillCopy"]?.ToString(),
                                    Status = reader["Status"]?.ToString()
                                });
                            }
                        }

                        return expenses;
                    }
                }
            }
        

//public List<Expenz> GetUserExpenses(string username)
//{
//    List<Expenz> expenses = new List<Expenz>();

//    using (SqlConnection connection = new SqlConnection(connectionString))
//    {
//        SqlCommand command = new SqlCommand("GetUserExpenses", connection)
//        {
//            CommandType = CommandType.StoredProcedure
//        };

//        // Add parameter for CreatedBy (which is the Username)
//        command.Parameters.AddWithValue("@CreatedBy", username);

//        connection.Open();
//        SqlDataReader reader = command.ExecuteReader();

//        while (reader.Read())
//        {
//            expenses.Add(new Expenz
//            {
//                Expenseid = Convert.ToInt32(reader["ExpenseID"]),
//                ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
//                Amount = Convert.ToInt32(reader["Amount"]),
//                DateofExpense = Convert.ToDateTime(reader["DateofExpense"]),
//                Description = reader["Description"]?.ToString(),
//                CreatedBy = reader["CreatedBy"]?.ToString(),
//                Status = reader["Status"]?.ToString(),
//                BillCopy = reader["BillCopy"]?.ToString()
//            });
//        }
//    }

//    return expenses;
//}

