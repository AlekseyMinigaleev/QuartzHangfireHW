using Npgsql;
using Quartz;
using Quartz.AspNetCore;
using Shared;

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
            StorageCreator.CreateDatabase(connectionString);
            CreateQuartzTables(connectionString);
        }

        private static void CreateQuartzTables(
            string connectionString)
        {
            var connectionStingBuilder =
                new NpgsqlConnectionStringBuilder(connectionString);
            var targetDatabase = connectionStingBuilder.Database!;

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