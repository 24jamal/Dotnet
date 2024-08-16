using Microsoft.Data.SqlClient;

namespace INTERNMvc.DAL
{
    public class UserRepository
    {

        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public (string Email, string Password) GetUserCredentials(string username)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Email, Password FROM UsersJ WHERE Username = @Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (reader["Email"].ToString(), reader["Password"].ToString());
                        }
                        else
                        {
                            throw new Exception("User not found.");
                        }
                    }
                }
            }

        }
    }
}