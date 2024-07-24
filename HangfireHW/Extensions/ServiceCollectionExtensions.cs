using Hangfire;
using Hangfire.PostgreSql;
using Shared;

namespace HangfireHW.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHangfire(
            this IServiceCollection services,
            string connectionString)
        {
            StorageCreator.CreateDatabase(connectionString);

            services
                .AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(connectionString)));

            services.AddHangfireServer();

            return services;
        }
    }
}