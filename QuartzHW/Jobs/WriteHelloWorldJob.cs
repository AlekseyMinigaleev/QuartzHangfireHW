using Quartz;

namespace QuartzHW.Jobs
{
    public class WriteHelloWorldJob : IJob
    {
        private static int _i;
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => Console.WriteLine(_i++));
        }
    }
}
