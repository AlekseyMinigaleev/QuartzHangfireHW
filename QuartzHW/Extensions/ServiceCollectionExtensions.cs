using Npgsql;
using Quartz;
using Quartz.AspNetCore;

namespace QuartzHW.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQuartz(
            this IServiceCollection services,
            string connectionString)
        {
            InitQuartzStorage(connectionString);

            services.AddQuartz(q =>
            {
                q.UsePersistentStore(opt =>
                {
                    opt.UsePostgres(connectionString);
                    opt.UseNewtonsoftJsonSerializer();
                });

                q.AddJobsAndTriggers();
            });

            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            return services;
        }

        private static void InitQuartzStorage(string connectionString)
        {
            var connectionStingBuilder =
                new NpgsqlConnectionStringBuilder(connectionString);
            var targetDatabase = connectionStingBuilder.Database!;

            CreateDatabase(connectionStingBuilder, targetDatabase);

            CreateQuartzTables(connectionStingBuilder, targetDatabase);
        }

        private static void CreateDatabase(
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

        private static void CreateQuartzTables(
            NpgsqlConnectionStringBuilder connectionStingBuilder,
            string targetDatabase)
        {
            connectionStingBuilder.Database = targetDatabase;
            using var targetConnection = new NpgsqlConnection(connectionStingBuilder.ConnectionString);
            targetConnection.Open();
            var scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQL", "tables_postgres.sql");
            ExecuteSqlScript(targetConnection, scriptPath);
        }

        private static void ExecuteSqlScript(NpgsqlConnection connection, string scriptPath)
        {
            var script = File.ReadAllText(scriptPath);

            using var scriptCmd = new NpgsqlCommand(script, connection);
            scriptCmd.ExecuteNonQuery();
        }
    }
}