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

        public List<Expenz> GetAllExpenses()
        {
            List<Expenz> expenses = new List<Expenz>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        e.Expenseid, 
                        e.ExpenseTypeID, 
                        et.ExpenseType, 
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
                        ExpenseTypeName = reader["ExpenseType"].ToString(), // Ensure this is not null
                        Amount = Convert.ToInt32(reader["Amount"]),
                        DateofExpense = Convert.ToDateTime(reader["DateofExpense"]),
                        Description = reader["Description"].ToString(),
                        CreatedBy = reader["CreatedBy"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                        BillCopy = reader["BillCopy"].ToString(),
                        Status = reader["Status"].ToString()
                    });

                }
            }

            return expenses;
        }
        public bool UpdateExpenz(int expenseid, int expenseTypeID, decimal amount, DateTime dateofExpense, string description, string billCopy, string status)
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

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }

        // Delete expense method
        public bool DeleteExpenz(int expenseid)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DeleteExpenz", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Expenseid", expenseid);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
        

    
    //// Get all Expense Types from the ExpenseType table
    //public List<SelectListItem> GetExpenseTypes()
    //{
    //    List<SelectListItem> expenseTypes = new List<SelectListItem>();
    //    using (var connection = new SqlConnection(connectionString))
    //    {
    //        string query = "SELECT ExpenseTypeID, ExpenseType FROM ExpenseTypes";
    //        SqlCommand command = new SqlCommand(query, connection);

//        connection.Open();
//        SqlDataReader reader = command.ExecuteReader();
//        while (reader.Read())
//        {
//            expenseTypes.Add(new SelectListItem
//            {
//                Value = reader["ExpenseTypeID"].ToString(),
//                Text = reader["ExpenseType"].ToString()
//            });
//        }
//    }
//    return expenseTypes;
//}

// Insert a new expense
//public void InsertExpenz(Expenz expenz)
//{
//    using (var connection = new SqlConnection(connectionString))
//    {
//        string query = "INSERT INTO Expenz (ExpenseTypeID, Amount, DateofExpense, Description, CreatedBy, CreatedAt, BillCopy, Status) " +
//                       "VALUES (@ExpenseTypeID, @Amount, @DateofExpense, @Description, @CreatedBy, @CreatedAt, @BillCopy, @Status)";

//        SqlCommand command = new SqlCommand(query, connection);
//        command.Parameters.AddWithValue("@ExpenseTypeID", expenz.ExpenseTypeID);
//        command.Parameters.AddWithValue("@Amount", expenz.Amount);
//        command.Parameters.AddWithValue("@DateofExpense", expenz.DateofExpense);
//        command.Parameters.AddWithValue("@Description", expenz.Description);
//        command.Parameters.AddWithValue("@CreatedBy", expenz.CreatedBy);
//        command.Parameters.AddWithValue("@CreatedAt", expenz.CreatedAt);
//        command.Parameters.AddWithValue("@BillCopy", expenz.BillCopy);
//        command.Parameters.AddWithValue("@Status", expenz.Status);

//        connection.Open();
//        command.ExecuteNonQuery();
//    }
//}

// Update an existing expense
//    public void UpdateExpenz(Expenz expenz)
//        {
//            using (var connection = new SqlConnection(connectionString))
//            {
//                string query = "UPDATE Expenz SET ExpenseTypeID = @ExpenseTypeID, Amount = @Amount, DateofExpense = @DateofExpense, " +
//                               "Description = @Description, BillCopy = @BillCopy, Status = @Status WHERE Expenseid = @Expenseid";

//                SqlCommand command = new SqlCommand(query, connection);
//                command.Parameters.AddWithValue("@Expenseid", expenz.Expenseid);
//                command.Parameters.AddWithValue("@ExpenseTypeID", expenz.ExpenseTypeID);
//                command.Parameters.AddWithValue("@Amount", expenz.Amount);
//                command.Parameters.AddWithValue("@DateofExpense", expenz.DateofExpense);
//                command.Parameters.AddWithValue("@Description", expenz.Description);
//                command.Parameters.AddWithValue("@BillCopy", expenz.BillCopy);
//                command.Parameters.AddWithValue("@Status", expenz.Status);

//                connection.Open();
//                command.ExecuteNonQuery();
//            }
//        }

//        // Delete an expense
//        public void DeleteExpenz(int expenseid)
//        {
//            using (var connection = new SqlConnection(connectionString))
//            {
//                string query = "DELETE FROM Expenz WHERE Expenseid = @Expenseid";
//                SqlCommand command = new SqlCommand(query, connection);
//                command.Parameters.AddWithValue("@Expenseid", expenseid);

//                connection.Open();
//                command.ExecuteNonQuery();
//            }
//        }
//    }
//}
//// Insert Expenz with file upload
//public bool InsertExpenz(Expenz expenz, FileStream stream)
//{
//    using (SqlConnection conn = new SqlConnection(connectionString))
//    using (SqlCommand cmd = new SqlCommand("InsertExpenz", conn))
//    {
//        cmd.CommandType = CommandType.StoredProcedure;

//        // Handling the file upload for BillCopy
//        string filePath = null;
//        if (expenz.BillCopy != null)
//        {
//            var fileName = Path.GetFileName(expenz.BillCopy.FileName);
//            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
//            filePath = Path.Combine(uploadsFolder, fileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                expenz.BillCopy.CopyTo(stream);
//            }
//        }

//        // Setting the parameters
//        cmd.Parameters.AddWithValue("@ExpenseTypeID", expenz.ExpenseTypeID);
//        cmd.Parameters.AddWithValue("@Amount", expenz.Amount);
//        cmd.Parameters.AddWithValue("@DateofExpense", expenz.DateofExpense);
//        cmd.Parameters.AddWithValue("@Description", expenz.Description);
//        cmd.Parameters.AddWithValue("@CreatedBy", expenz.CreatedBy);
//        cmd.Parameters.AddWithValue("@CreatedAt", expenz.CreatedAt);
//        cmd.Parameters.AddWithValue("@BillCopy", filePath ?? string.Empty); // If no file, pass empty string
//        cmd.Parameters.AddWithValue("@Status", expenz.Status);

//        conn.Open();
//        return cmd.ExecuteNonQuery() > 0; // Return true if the insertion is successful
//    }
//}

// Get all expense types for dropdown
//public List<ExpenseTypes> GetAllExpenseTypes()
//        {
//            var expenseTypes = new List<ExpenseTypes>();
//            using (var connection = new SqlConnection(connectionString))
//            {
//                var command = new SqlCommand("GetAllExpenseTypes", connection)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };

//                connection.Open();
//                using (var reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        expenseTypes.Add(new ExpenseTypes
//                        {
//                            ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
//                            ExpenseType = reader["ExpenseType"].ToString()
//                        });
//                    }
//                }
//            }
//            return expenseTypes;
//        }

//public bool InsertExpenz(Expenz expenz)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            using (SqlCommand cmd = new SqlCommand("InsertExpenz", conn))
//            {
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@ExpenseTypeID", expenz.ExpenseTypeID);
//                cmd.Parameters.AddWithValue("@Amount", expenz.Amount);
//                cmd.Parameters.AddWithValue("@DateofExpense", expenz.DateofExpense);
//                cmd.Parameters.AddWithValue("@Description", expenz.Description);
//                cmd.Parameters.AddWithValue("@CreatedBy", expenz.CreatedBy);
//                cmd.Parameters.AddWithValue("@CreatedAt", expenz.CreatedAt);
//                cmd.Parameters.AddWithValue("@BillCopy", expenz.BillCopy);
//                cmd.Parameters.AddWithValue("@Status", expenz.Status);

//                conn.Open();
//                return cmd.ExecuteNonQuery() > 0;
//            }
//        }
//        public bool UpdateExpenz(Expenz expenz)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            using (SqlCommand cmd = new SqlCommand("UpdateExpenz", conn))
//            {
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@Expenseid", expenz.Expenseid);
//                cmd.Parameters.AddWithValue("@ExpenseTypeID", expenz.ExpenseTypeID);
//                cmd.Parameters.AddWithValue("@Amount", expenz.Amount);
//                cmd.Parameters.AddWithValue("@DateofExpense", expenz.DateofExpense);
//                cmd.Parameters.AddWithValue("@Description", expenz.Description);
//                cmd.Parameters.AddWithValue("@BillCopy", expenz.BillCopy);
//                cmd.Parameters.AddWithValue("@Status", expenz.Status);

//                conn.Open();
//                return cmd.ExecuteNonQuery() > 0;
//            }
//        }

//        public bool DeleteExpenz(int expenseId)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            using (SqlCommand cmd = new SqlCommand("DeleteExpenz", conn))
//            {
//                cmd.CommandType = CommandType.StoredProcedure;
//                cmd.Parameters.AddWithValue("@Expenseid", expenseId);

//                conn.Open();
//                return cmd.ExecuteNonQuery() > 0;
//            }
//        }


//      public List<Expense> GetAllExpenses()
//        {
//            var expenses = new List<Expense>();
//            using (var connection = new SqlConnection(connectionString))
//            {
//                var command = new SqlCommand("GetAllExpenses", connection)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };

//                connection.Open();
//                using (var reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        expenses.Add(new Expense
//                        {
//                            ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
//                            ExpenseType = reader["ExpenseType"].ToString(),
//                            Amount = Convert.ToDecimal(reader["Amount"]),
//                            DateOfExpense = Convert.ToDateTime(reader["DateOfExpense"]),
//                            Description = reader["Description"].ToString(),
//                            CreatedBy = reader["CreatedBy"].ToString(),
//                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
//                        });
//                    }
//                }
//            }
//            return expenses;
//        }

//        // Get expense by ID
//        public Expense GetExpenseByID(int expenseID)
//        {
//            Expense expense = null;
//            using (var connection = new SqlConnection(connectionString))
//            {
//                var command = new SqlCommand("GetExpenseByID", connection)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };
//                command.Parameters.AddWithValue("@ExpenseID", expenseID);

//                connection.Open();
//                using (var reader = command.ExecuteReader())
//                {
//                    if (reader.Read())
//                    {
//                        expense = new Expense
//                        {
//                            ExpenseID = Convert.ToInt32(reader["ExpenseID"]),
//                            ExpenseType = reader["ExpenseType"].ToString(),
//                            Amount = Convert.ToDecimal(reader["Amount"]),
//                            DateOfExpense = Convert.ToDateTime(reader["DateOfExpense"]),
//                            Description = reader["Description"].ToString(),
//                            CreatedBy = reader["CreatedBy"].ToString(),
//                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
//                        };
//                    }
//                }
//            }
//            return expense;
//        }
//        public void AddExpense(Expense expense)
//        {
//            using (var connection = new SqlConnection(connectionString))
//            {
//                var command = new SqlCommand("InsertExpense", connection)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };
//                command.Parameters.AddWithValue("@ExpenseType", expense.ExpenseType );
//                command.Parameters.AddWithValue("@Amount", expense.Amount);
//                command.Parameters.AddWithValue("@DateOfExpense", expense.DateOfExpense);
//                command.Parameters.AddWithValue("@Description", expense.Description );
//                command.Parameters.AddWithValue("@CreatedBy", expense.CreatedBy );
//                command.Parameters.AddWithValue("@BillCopyPath", expense.BillCopyPath );

//                connection.Open();
//                command.ExecuteNonQuery();
//            }
//        }


//        public void UpdateExpense(Expense expense)
//        {
//            using (var connection = new SqlConnection(connectionString))
//            {
//                var command = new SqlCommand("UpdateExpense", connection)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };
//                command.Parameters.AddWithValue("@ExpenseID", expense.ExpenseID);
//                command.Parameters.AddWithValue("@ExpenseType", expense.ExpenseType);
//                command.Parameters.AddWithValue("@Amount", expense.Amount);
//                command.Parameters.AddWithValue("@DateOfExpense", expense.DateOfExpense);
//                command.Parameters.AddWithValue("@Description", expense.Description);
//                command.Parameters.AddWithValue("@CreatedBy", expense.CreatedBy);
//                command.Parameters.AddWithValue("@BillCopyPath", expense.BillCopyPath);

//                connection.Open();
//                command.ExecuteNonQuery();
//            }
//        }


//        // Delete expense
//        public void DeleteExpense(int expenseID)
//        {
//            using (var connection = new SqlConnection(connectionString))
//            {
//                var command = new SqlCommand("DeleteExpense", connection)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };
//                command.Parameters.AddWithValue("@ExpenseID", expenseID);

//                connection.Open();
//                command.ExecuteNonQuery();
//            }
//        }
//        public List<ExpenseTypes> GetAllExpenseTypes()
//        {
//            var expenseTypes = new List<ExpenseTypes>();
//            using (var connection = new SqlConnection(connectionString))
//            {
//                var command = new SqlCommand("GetAllExpenseTypes", connection)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };

//                connection.Open();
//                using (var reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        expenseTypes.Add(new ExpenseTypes
//                        {
//                            ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
//                            ExpenseType = reader["ExpenseType"].ToString()
//                        });
//                    }
//                }
//            }
//            return expenseTypes;
//        }

//    }
//}
