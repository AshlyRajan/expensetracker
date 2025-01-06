using expensetracker.DAL;
using expensetracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MongoDB.Driver.Core.Configuration;
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

        public void InsertRegistration(signup signup)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CreateSignup", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", signup.Name);
                    cmd.Parameters.AddWithValue("@Address", signup.Address);
                    cmd.Parameters.AddWithValue("@State", signup.State);
                    cmd.Parameters.AddWithValue("@District", signup.District);
                    cmd.Parameters.AddWithValue("@Gender", signup.Gender);
                    cmd.Parameters.AddWithValue("@Email", signup.Email);
                    cmd.Parameters.AddWithValue("@Phone", signup.Phone);
                    cmd.Parameters.AddWithValue("@Username", signup.Username);
                    cmd.Parameters.AddWithValue("@Password", signup.Password);
                    cmd.Parameters.AddWithValue("@Confirmpassword", signup.ConfirmPassword);
                    cmd.Parameters.AddWithValue("@Role", signup.Role);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Registration inserted successfully.");
                    }
                    catch (Exception ex)
                    {
                        // Log or rethrow the exception as needed
                        Console.WriteLine($"Error inserting registration: {ex.Message}");
                        throw;
                    }
                }
            }
        }



        public signup GetSignupById(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetSignupById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
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
                                Username = reader["Username"].ToString(),
                                //Password = reader["Password"].ToString(),
                                //ConfirmPassword = reader["Confirmpassword"].ToString(),
                                Role = reader["Role"].ToString()
                            };
                        }
                        return null;
                    }
                }
            }
        }

        public void UpdateSignup(signup user)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateSignup", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@State", user.State);
                    cmd.Parameters.AddWithValue("@District", user.District);
                    cmd.Parameters.AddWithValue("@Gender", user.Gender);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Username", user.Username);
                    cmd.Parameters.AddWithValue("@Role", user.Role);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("User information updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception
                        Console.WriteLine($"Error updating user information: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public void DeleteSignup(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteSignup", conn))
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
                        // Log or handle the exception
                        Console.WriteLine($"Error deleting user with ID {id}: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public signup GetUserDetails(int userId)
{
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetSignupById", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", userId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
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
                                    Username = reader["Username"].ToString(),
                                    Role = reader["Role"].ToString()
                                };
                            }
                            else
                            {
                                // Log or handle the case when no data is returned
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error message to understand what went wrong
                Console.WriteLine(ex.Message);
            }

        
    return null;
}

       
        public Registration Login(string Username, string Password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("signin", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@Password", Password);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            return new Registration
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Role = reader.GetString(2),
                                Username = Username,  // Explicitly set the Username
                                Password = Password   // Explicitly set the Password
                            };
                        }
                    }
                }
            }
            return null; // Return null if no user is found
        }

        public void InsertContactMessage(ContactMessage model)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("InsertContactMessage", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Adding parameters to the stored procedure
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Message", model.Message);

                // Execute the stored procedure
                cmd.ExecuteNonQuery();
            }
        }

        // Get all contact messages from the database using the stored procedure
        public List<ContactMessage> GetAllContactMessages()
        {
            List<ContactMessage> messages = new List<ContactMessage>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("GetAllContactMessages", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = cmd.ExecuteReader();
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

            return messages;
        }

        // Delete a contact message by its Id using the stored procedure
        public void DeleteContactMessage(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("DeleteContactMessage", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Id", id);

                // Execute the stored procedure
                cmd.ExecuteNonQuery();
            }
        }

    }
}

//        public Registration Login(string Email, string Password)
//        {
//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("signin", conn))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@Email", Email);
//                    cmd.Parameters.AddWithValue("@Password", Password);
//                    conn.Open();
//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        if (reader.HasRows)
//                        {
//                            reader.Read();
//                            return new Registration
//                            {
//                                Id = reader.GetInt32(0),
//                                Name = reader.GetString(1),
//                                Role = reader.GetString(2),
//                                Email = Email, // Explicitly set the Email 
//                                Password = Password // Explicitly set the Password 
//                            };
//                        }
//                    }
//                }
//            }
//            return null; // Return null if no user is found 
//        }
//    }
//}