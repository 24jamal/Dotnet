using INTERNMvc.Models;
using Microsoft.Data.SqlClient;

namespace INTERNMvc.DAL
{
    public class UserDAL
    {
        private readonly string connectionString;

        public UserDAL(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DbContext");
        }

        public bool RegisterUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO UsersJ (Username, Password, Email) VALUES (@Username, @Password, @Email)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Email", user.Email);

                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        public User ValidateUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM UsersJ WHERE Username = @Username AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        UserId = Convert.ToInt32(reader["UserId"]),
                        Username = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        Email = reader["Email"].ToString()
                    };
                }
                else
                {
                    return null;
                }
            }
        }

    }
}
