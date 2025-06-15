using Npgsql;

namespace HuskKeyInfra.Database
{
    public class DatabaseConfiguration
    {
        private readonly string _connectionString;
        public string ConnectionString => _connectionString;

        public DatabaseConfiguration(string host, string username, string password, string databaseName, int port = 5432, bool pooling = true)
        {
            if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException("Host cannot be null or empty.", nameof(host));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username cannot be null or empty.", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("Database name cannot be null or empty.", nameof(databaseName));

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Username = username,
                Password = password,
                Database = databaseName,
                Port = port,
                Pooling = pooling, // Enable connection pooling

            };
            _connectionString = builder.ConnectionString;
        }

        public DatabaseConfiguration(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            _connectionString = connectionString;
        }
    }
}
