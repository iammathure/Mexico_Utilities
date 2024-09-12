using Microsoft.Data.SqlClient;

namespace Mexico_Utility.DBConnect
{
    public class DatabaseConnection
    {
        private readonly string connectionString;

        public DatabaseConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                {
                    throw new ArgumentNullException(nameof(connectionString), "The connection string cannot be null or empty.");
                }
            }
            this.connectionString = connectionString;
        }

        public async Task<SqlConnection> GetConnectionAsync()
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
