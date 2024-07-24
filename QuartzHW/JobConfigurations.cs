using Quartz;
using QuartzHW.Jobs;

namespace QuartzHW
{
    public static class JobConfiguration
    {
        private static readonly JobKey FirstJobKey = new($"{nameof(HelloWorldJob)}-first");
        private static readonly TriggerKey FirstTriggerKey = new($"{nameof(HelloWorldJob)}-trigger-first");

        private static readonly JobKey SecondJobKey = new($"{nameof(HelloWorldJob)}-second");
        private static readonly TriggerKey SecondTriggerKey = new($"{nameof(HelloWorldJob)}-trigger-second");

        public static readonly List<JobInfo> Jobs =
        [
            new JobInfo
            {
                JobKey = FirstJobKey,

                TriggerKey = new TriggerKey(FirstJobKey.Name),

                JobDetail = JobBuilder.Create<HelloWorldJob>()
                    .WithIdentity(FirstJobKey)
                    .UsingJobData(nameof(HelloWorldJob.Count), 0)
                    .UsingJobData(nameof(HelloWorldJob.JobName), FirstJobKey.Name)
                    .StoreDurably()
                    .Build(),

                Triggers =
                [
                    TriggerBuilder.Create()
                        .ForJob(FirstJobKey)
                        .WithIdentity(FirstTriggerKey)
                        .WithSimpleSchedule(x => x
                            .WithInterval(TimeSpan.FromSeconds(1))
                            .RepeatForever())
                        .Build()
                ]
            },

            new JobInfo
            {
                JobKey = SecondJobKey,
                TriggerKey = SecondTriggerKey,

                JobDetail = JobBuilder.Create<HelloWorldJob>()
                    .WithIdentity(SecondJobKey)
                    .UsingJobData(nameof(HelloWorldJob.Count), 100)
                    .UsingJobData(nameof(HelloWorldJob.JobName), SecondJobKey.Name)
                 .StoreDurably()
                    .Build(),

                Triggers =
                [
                    TriggerBuilder.Create()
                        .ForJob(SecondJobKey)
                        .WithIdentity(SecondTriggerKey)
                        .WithSimpleSchedule(x => x
                            .WithInterval(TimeSpan.FromSeconds(1))
                            .RepeatForever())
                        .Build()
                ]
            }
        ];
    }
}