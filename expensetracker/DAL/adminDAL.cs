using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using expensetracker.Models;

namespace expensetracker.DAL
{
    public class adminDAL
    {
        private readonly string connectionString;

        public adminDAL(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public void AddExpenseType(string ExpenseTypes)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("InsertExpenseType", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ExpenseType", ExpenseTypes);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        public List<Expenz> GetAllExpenses()
        {
            List<Expenz> expenses = new List<Expenz>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("GetExpensesWithApprovedAmount", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
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
                            CreatedBy = reader["CreatedBy"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            Status = reader["Status"].ToString(),
                            ApprovedAmount = Convert.ToInt32(reader["ApprovedAmount"]),
                            ApprovedDate = Convert.ToDateTime(reader["ApprovedDate"])
                        });
                    }
                }
            }

            return expenses;
        }


        public List<ExpenseTypes> GetAllExpenseTypes()
        {
            var expenseTypes = new List<ExpenseTypes>();
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("GetAllExpenseTypes", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        expenseTypes.Add(new ExpenseTypes
                        {
                            ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                            ExpenseType = reader["ExpenseType"].ToString()
                        });
                    }
                }
            }
            return expenseTypes;
        }

        public ExpenseTypes GetExpenseTypeById(int id)
        {
            ExpenseTypes expenseType = null;
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("GetExpenseTypeById", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ExpenseTypeID", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        expenseType = new ExpenseTypes
                        {
                            ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
                            ExpenseType = reader["ExpenseType"].ToString()
                        };
                    }
                }
            }
            return expenseType;
        }

        public void UpdateExpenseType(int id, string ExpenseType)

        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("UpdateExpenseType", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ExpenseTypeID", id);
                command.Parameters.AddWithValue("@ExpenseType", ExpenseType);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteExpenseType(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("DeleteExpenseType", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@ExpenseTypeID", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


        public void UpdateExpenseStatus(int expenseId, string status, int? approvedAmount = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateExpenseStatus", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@ExpenseId", expenseId);
                cmd.Parameters.AddWithValue("@Status", status);
                if (approvedAmount.HasValue)
                    cmd.Parameters.AddWithValue("@ApprovedAmount", approvedAmount.Value);
                else
                    cmd.Parameters.AddWithValue("@ApprovedAmount", DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        // Method to get expenses with approved amounts by calling the stored procedure
        public List<ExpenseModel> GetExpensesWithApprovedAmount()
        {
            var expenses = new List<ExpenseModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetExpensesWithApprovedAmount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            expenses.Add(new ExpenseModel
                            {
                                Expenseid = reader.GetInt32(reader.GetOrdinal("Expenseid")),
                                ExpenseTypeID = reader.GetInt32(reader.GetOrdinal("ExpenseTypeID")),
                                Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
                                DateofExpense = reader.GetDateTime(reader.GetOrdinal("DateofExpense")),
                                //Description = reader.GetString(reader.GetOrdinal("Description")),
                                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
                                Createdat = reader.GetDateTime(reader.GetOrdinal("Createdat")),
                                //Billcopy = reader.GetString(reader.GetOrdinal("Billcopy")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                ApprovedAmount = reader.GetInt32(reader.GetOrdinal("ApprovedAmount")),
                                ApprovedDate = reader.GetDateTime(reader.GetOrdinal("ApprovedDate"))
                            });
                        }
                    }
                }
            }

            return expenses;
        }
        public void UpdateSignup(signup signup)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("UpdateSignup", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", signup.Id);
                cmd.Parameters.AddWithValue("@Name", signup.Name);
                cmd.Parameters.AddWithValue("@Address", signup.Address);
                cmd.Parameters.AddWithValue("@State", signup.State);
                cmd.Parameters.AddWithValue("@District", signup.District);
                cmd.Parameters.AddWithValue("@Gender", signup.Gender);
                cmd.Parameters.AddWithValue("@Email", signup.Email);
                cmd.Parameters.AddWithValue("@Phone", signup.Phone);
                cmd.Parameters.AddWithValue("@Password", signup.Password);
                cmd.Parameters.AddWithValue("@Confirmpassword", signup.ConfirmPassword);
                cmd.Parameters.AddWithValue("@Role", signup.Role);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"User with ID {signup.Id} updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating user with ID {signup.Id}: {ex.Message}");
                    throw;
                }
            }
        }


        public void DeleteSignup(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("DeleteSignup", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine($"User with ID {id} deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting user with ID {id}: {ex.Message}");
                    throw;
                }
            }
        }


        public signup GetSignupById(int id)
        {
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM signup WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new signup
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Address = reader["Address"].ToString(),
                            State = reader["State"].ToString(),
                            District = reader["District"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Password = reader["Password"].ToString(),
                            ConfirmPassword = reader["Confirmpassword"].ToString(),
                            Role = reader["Role"].ToString()
                        };
                    }
                }
            }
            return null;
        }
        public List<signup> GetAllSignups()
        {
            var signups = new List<signup>();

            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT * FROM signup", conn))
            {
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        signups.Add(new signup
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            Address = reader["Address"].ToString(),
                            State = reader["State"].ToString(),
                            District = reader["District"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Password = reader["Password"].ToString(),
                            ConfirmPassword = reader["Confirmpassword"].ToString(),
                            Role = reader["Role"].ToString()
                        });
                    }
                }
            }

            return signups;
        }

        public async Task<List<Expenz>> GetAllExpenzAsync()
        {
            var expenses = new List<Expenz>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("GetAllExpenz", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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
                                BillCopy = reader.IsDBNull(reader.GetOrdinal("Billcopy")) ? null : reader.GetString(reader.GetOrdinal("Billcopy")),
                                Status = reader.GetString(reader.GetOrdinal("Status"))
                            };

                            expenses.Add(expense);
                        }
                    }
                }
            }

            return expenses;
        }
        // Fetch all contact messages
        public List<ContactMessage> GetAllContactMessages()
        {
            var messages = new List<ContactMessage>();
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("SELECT * FROM ContactMessages", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(new ContactMessage
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Message = reader["Message"].ToString(),
                            SubmittedAt = (DateTime)reader["SubmittedAt"]
                        });
                    }
                }
            }
            return messages;
        }

        // Fetch a single contact message by ID
        public ContactMessage GetContactMessageById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("SELECT * FROM ContactMessages WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ContactMessage
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Message = reader["Message"].ToString(),
                            SubmittedAt = (DateTime)reader["SubmittedAt"]
                        };
                    }
                }
            }
            return null;
        }

        // Delete a contact message
        public void DeleteContactMessage(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("DELETE FROM ContactMessages WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}

//        public async Task<List<Expenz>> GetUserExpensesAsync(string createdBy)
//        {
//            List<Expenz> expenses = new List<Expenz>();

//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                await connection.OpenAsync();

//                using (SqlCommand command = new SqlCommand("GetUserExpenses", connection))
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.Parameters.AddWithValue("@CreatedBy", createdBy);

//                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
//                    {
//                        while (await reader.ReadAsync())
//                        {
//                            expenses.Add(new Expenz
//                            {
//                                Expenseid = reader.GetInt32(reader.GetOrdinal("ExpenseID")),
//                                ExpenseTypeID = reader.GetInt32(reader.GetOrdinal("ExpenseTypeID")),
//                                Amount = reader.GetInt32(reader.GetOrdinal("Amount")),
//                                DateofExpense = reader.GetDateTime(reader.GetOrdinal("DateofExpense")),
//                                Description = reader.GetString(reader.GetOrdinal("Description")),
//                                CreatedBy = reader.GetString(reader.GetOrdinal("CreatedBy")),
//                                Status = reader.GetString(reader.GetOrdinal("Status")),
//                                BillCopy = reader.IsDBNull(reader.GetOrdinal("BillCopy")) ? null : reader.GetString(reader.GetOrdinal("BillCopy"))
//                            });
//                        }
//                    }
//                }
//            }

//            return expenses;
//        }


//    }

//}


////employees and admins   
//public List<Registration> GetAllEmployees()
//{
//    var employees = new List<Registration>();
//    using (var connection = new SqlConnection(connectionString))
//    {
//        var command = new SqlCommand("SELECT * FROM registration", connection);
//        connection.Open();
//        using (var reader = command.ExecuteReader())
//        {
//            while (reader.Read())
//            {
//                employees.Add(new Registration
//                {
//                    Id = (int)reader["id"],
//                    Name = reader["Name"].ToString(),
//                    Age = (int)reader["Age"],
//                    Address = reader["Address"].ToString(),
//                    Email = reader["Email"].ToString(),
//                    Username = reader["username"].ToString(),
//                    Password = reader["password"].ToString(),
//                    confirmpassword = reader["confirmpassword"].ToString(),
//                    Role = reader["role"].ToString()
//                });
//            }
//        }
//    }
//    return employees;
//}

//public Registration GetEmployeeById(int id)
//{
//    Registration employee = null;
//    using (var connection = new SqlConnection(connectionString))
//    {
//        var command = new SqlCommand("SELECT * FROM registration WHERE id = @Id", connection);
//        command.Parameters.AddWithValue("@Id", id);
//        connection.Open();
//        using (var reader = command.ExecuteReader())
//        {
//            if (reader.Read())
//            {
//                employee = new Registration
//                {
//                    Id = (int)reader["id"],
//                    Name = reader["Name"].ToString(),
//                    Age = (int)reader["Age"],
//                    Address = reader["Address"].ToString(),
//                    Email = reader["Email"].ToString(),
//                    Username = reader["username"].ToString(),
//                    Password = reader["password"].ToString(),
//                    confirmpassword = reader["confirmpassword"].ToString(),
//                    Role = reader["role"].ToString()
//                };
//            }
//        }
//    }
//    return employee;
//}

//public void AddEmployee(Registration employee)
//{
//    using (var connection = new SqlConnection(connectionString))
//    {
//        var command = new SqlCommand(
//            "INSERT INTO registration (Name, Age, Address, Email, username, password, confirmpassword, role) " +
//            "VALUES (@Name, @Age, @Address, @Email, @Username, @Password, @ConfirmPassword, @Role)",
//            connection);

//        command.Parameters.AddWithValue("@Name", employee.Name);
//        command.Parameters.AddWithValue("@Age", employee.Age);
//        command.Parameters.AddWithValue("@Address", employee.Address);
//        command.Parameters.AddWithValue("@Email", employee.Email);
//        command.Parameters.AddWithValue("@Username", employee.Username);
//        command.Parameters.AddWithValue("@Password", employee.Password);
//        command.Parameters.AddWithValue("@ConfirmPassword", employee.confirmpassword);
//        command.Parameters.AddWithValue("@Role", employee.Role);

//        connection.Open();
//        command.ExecuteNonQuery();
//    }
//}

//public void UpdateEmployee(int id, Registration employee)
//{
//    using (var connection = new SqlConnection(connectionString))
//    {
//        var command = new SqlCommand(
//            "UPDATE registration SET Name = @Name, Age = @Age, Address = @Address, Email = @Email, " +
//            "username = @Username, password = @Password, confirmpassword = @ConfirmPassword, role = @Role WHERE id = @Id",
//            connection);

//        command.Parameters.AddWithValue("@Id", id);
//        command.Parameters.AddWithValue("@Name", employee.Name);
//        command.Parameters.AddWithValue("@Age", employee.Age);
//        command.Parameters.AddWithValue("@Address", employee.Address);
//        command.Parameters.AddWithValue("@Email", employee.Email);
//        command.Parameters.AddWithValue("@Username", employee.Username);
//        command.Parameters.AddWithValue("@Password", employee.Password);
//        command.Parameters.AddWithValue("@ConfirmPassword", employee.confirmpassword);
//        command.Parameters.AddWithValue("@Role", employee.Role);

//        connection.Open();
//        command.ExecuteNonQuery();
//    }
//}

//public void DeleteEmployee(int id)
//{
//    using (var connection = new SqlConnection(connectionString))
//    {
//        var command = new SqlCommand("DELETE FROM registration WHERE id = @Id", connection);
//        command.Parameters.AddWithValue("@Id", id);
//        connection.Open();
//        command.ExecuteNonQuery();
//    }
//}

//public List<Expenz> GetAllExpenses()
//{
//    List<Expenz> expenses = new List<Expenz>();

//    using (SqlConnection connection = new SqlConnection(connectionString))
//    {
//        string query = @"
//            SELECT 
//                e.Expenseid, 
//                e.ExpenseTypeID, 
//                et.ExpenseType, 
//                e.Amount, 
//                e.DateofExpense, 
//                e.Description, 
//                e.CreatedBy, 
//                e.CreatedAt, 
//                e.BillCopy, 
//                e.Status
//            FROM 
//                Expenz e
//            JOIN 
//                ExpenseTypes et ON e.ExpenseTypeID = et.ExpenseTypeID";

//        SqlCommand command = new SqlCommand(query, connection);
//        connection.Open();

//        SqlDataReader reader = command.ExecuteReader();
//        while (reader.Read())
//        {
//            expenses.Add(new Expenz
//            {
//                Expenseid = Convert.ToInt32(reader["Expenseid"]),
//                ExpenseTypeID = Convert.ToInt32(reader["ExpenseTypeID"]),
//                ExpenseTypeName = reader["ExpenseType"].ToString(), // Ensure this is not null
//                Amount = Convert.ToInt32(reader["Amount"]),
//                DateofExpense = Convert.ToDateTime(reader["DateofExpense"]),
//                Description = reader["Description"].ToString(),
//                CreatedBy = reader["CreatedBy"].ToString(),
//                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
//                BillCopy = reader["BillCopy"].ToString(),
//                Status = reader["Status"].ToString()
//            });

//        }
//    }

//    return expenses;
//}