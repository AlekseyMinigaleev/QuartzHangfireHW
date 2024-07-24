using Hangfire;
using HangfireHW.Jobs;

namespace HangfireHW
{
    public static class JobConfiguration
    {
        private const string FirstJobId = $"{nameof(HelloWorldJob)}-first";
        private const string SecondJobId = $"{nameof(HelloWorldJob)}-second";

        public static void RegisterAllJobs()
        {
            AddRecurringJobs();
        }

        public static void AddRecurringJobs()
        {
            RecurringJob.AddOrUpdate<HelloWorldJob>(
                recurringJobId: $"[recurring]{FirstJobId}",
                methodCall: (job) => job.ExecuteAsync(
                    $"[recurring]{FirstJobId}",
                    null,
                    CancellationToken.None),
                cronExpression: Cron.Never()
                );

            RecurringJob.AddOrUpdate<HelloWorldJob>(
                recurringJobId: $"[recurring]{SecondJobId}",
                methodCall: (job) => job.ExecuteAsync(
                    $"[recurring]{SecondJobId}",
                    null,
                    CancellationToken.None),
                cronExpression: Cron.Never()
            );
        }
    }
}