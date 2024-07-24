using Newtonsoft.Json;
using Quartz;

namespace QuartzHW.Jobs
{
    [PersistJobDataAfterExecution]
    public class HelloWorldJob : IJob
    {
        public int Count { private get; set; }

        public string JobName { private get; set; }

        public async Task Execute(IJobExecutionContext context)
        {
            Count++;

            var a = new
            {
                JobName,
                Count,
            };

            await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(a));

            //по какой то причине не работает, если сохранять данные в контексте тригера,
            //сохраняются только в контексте JobDetail,
            context.JobDetail.JobDataMap.Put(nameof(Count), Count);
            context.JobDetail.JobDataMap.Put(nameof(JobName), JobName);
        }
    }
}