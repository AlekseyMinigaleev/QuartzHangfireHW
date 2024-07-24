using Quartz;
using QuartzHW.Jobs;

namespace QuartzHW.Extensions
{
    public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        private static readonly string _firstHWJobKey = $"{HelloWorldJob.Key}-first";
        private static readonly string _secondHWJobKey = $"{HelloWorldJob.Key}-second";

        public static IServiceCollectionQuartzConfigurator AddJobs(
            this IServiceCollectionQuartzConfigurator quartzServices)
        {
            quartzServices
                .AddJob<HelloWorldJob>(opt => opt
                    .WithIdentity(_firstHWJobKey)
                    .UsingJobData(nameof(HelloWorldJob.Count), 0)
                    .UsingJobData(nameof(HelloWorldJob.JobName), _firstHWJobKey)
                );

            quartzServices
                .AddJob<HelloWorldJob>(opt => opt
                    .WithIdentity(_secondHWJobKey)
                    .UsingJobData(nameof(HelloWorldJob.Count), 100)
                    .UsingJobData(nameof(HelloWorldJob.JobName), _secondHWJobKey)
            );

            return quartzServices;
        }

        public static IServiceCollectionQuartzConfigurator AddTriggers(
            this IServiceCollectionQuartzConfigurator quartzServices)
        {
            quartzServices
                .AddTrigger(opt => opt
                    .ForJob(_firstHWJobKey)
                    .WithIdentity(
                        $"{HelloWorldJob.Key}-firstTrigger",
                        HelloWorldJob.Key.Group)
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromSeconds(1))
                        .RepeatForever())
                );

            quartzServices
                .AddTrigger(opt => opt
                    .ForJob(_secondHWJobKey)
                    .WithIdentity(
                        $"{HelloWorldJob.Key}-secondTrigger",
                        HelloWorldJob.Key.Group)
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromSeconds(1))
                        .RepeatForever())
                );

            return quartzServices;
        }
    }
}