using Hangfire;
using Hangfire.Server;
using Newtonsoft.Json;

namespace HangfireHW.Jobs
{
    public class HelloWorldJob() : IJob
    {
        private int Count { get; set; }
        private string JobName { get; set; }

        [AutomaticRetry(Attempts = int.MaxValue, DelaysInSeconds = [0])]
        public async Task ExecuteAsync(
            string jobName,
            PerformContext performContext,
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                JobName = jobName;

                Count++;

                var response = new
                {
                    JobName,
                    Count,
                };

                await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(response));

                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);

                if (Count == 100)
                    throw new Exception();
            }
        }
    }
}
