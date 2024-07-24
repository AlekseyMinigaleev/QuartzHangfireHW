using Npgsql;

namespace Shared
{
    public static class StorageCreator
    {
        public static void CreateDatabase(string connectionString)
        {
            var connectionStingBuilder =
                new NpgsqlConnectionStringBuilder(connectionString);
            var targetDatabase = connectionStingBuilder.Database!;

            ExecuteSQL(connectionStingBuilder, targetDatabase);
        }

        private static void ExecuteSQL(
            NpgsqlConnectionStringBuilder connectionStingBuilder,
            string targetDatabase)
        {
            connectionStingBuilder.Database = "postgres";

            using var connection = new NpgsqlConnection(connectionStingBuilder.ConnectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{targetDatabase}'", connection);
            var exists = cmd.ExecuteScalar() != null;

            if (!exists)
            {
                using var createCmd = new NpgsqlCommand($"CREATE DATABASE \"{targetDatabase}\"", connection);
                createCmd.ExecuteNonQuery();
            }
        }
    }
}
