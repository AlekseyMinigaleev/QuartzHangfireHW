using Quartz;

namespace QuartzHW.Extensions
{
    public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        public static IServiceCollectionQuartzConfigurator AddJobsAndTriggers(
        this IServiceCollectionQuartzConfigurator quartzServices)
        {
            foreach (var jobInfo in JobConfiguration.Jobs)
            {
                quartzServices
                    .AddJob(
                        jobInfo.JobDetail.JobType,
                        jobInfo.JobKey,
                        opt => opt
                            .WithIdentity(jobInfo.JobKey)
                            .SetJobData(jobInfo.JobDetail.JobDataMap)
                            .DisallowConcurrentExecution(jobInfo.JobDetail.ConcurrentExecutionDisallowed)
                            .PersistJobDataAfterExecution(jobInfo.JobDetail.PersistJobDataAfterExecution)
                            .RequestRecovery(jobInfo.JobDetail.RequestsRecovery)
                            .StoreDurably(jobInfo.JobDetail.Durable)
                            .WithDescription(jobInfo.JobDetail.Description)
                    );

                foreach (var trigger in jobInfo.Triggers)
                    quartzServices
                        .AddTrigger(opt =>opt
                            .ForJob(jobInfo.JobKey)
                            .WithIdentity(trigger.Key)
                            .WithSchedule(trigger.GetScheduleBuilder())
                            .EndAt(trigger.EndTimeUtc)
                            .ModifiedByCalendar(trigger.CalendarName)
                            .StartAt(trigger.StartTimeUtc)
                            .WithDescription(trigger.Description)
                            .WithPriority(trigger.Priority)
                            .WithSchedule(trigger.GetScheduleBuilder())
                    );
            }

            return quartzServices;
        }
    }
}