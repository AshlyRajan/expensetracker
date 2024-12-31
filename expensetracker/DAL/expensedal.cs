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

        public bool InsertRegistration(signup signup)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CreateSignup", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    
                    cmd.Parameters.AddWithValue("@Name", signup.Name);
                    cmd.Parameters.AddWithValue("@Address", signup.Address);
                    cmd.Parameters.AddWithValue("@State", signup.State);
                    cmd.Parameters.AddWithValue("@District", signup.District);
                    cmd.Parameters.AddWithValue("@Gender", signup.Gender);
                    cmd.Parameters.AddWithValue("@Email", signup.Email);
                    cmd.Parameters.AddWithValue("@Phone", signup.Phone);
                    cmd.Parameters.AddWithValue("@Password", signup.Password);
                    cmd.Parameters.AddWithValue("@ConfirmPassword", signup.ConfirmPassword);
                    cmd.Parameters.AddWithValue("@Role", signup.Role);

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();

                    return rowsAffected > 0;
                }
            }
        }
        public signup GetSignupById(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetSignupById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new signup
                            {
                                Id = reader["Id"].ToString(),
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
                        return null;
                    }
                }
            }
        }

        public bool UpdateSignup(signup signup)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateSignup", conn))
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

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool DeleteSignup(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteSignup", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public Registration Login(string Username, string Password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Login", conn))
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
    }
}

//     