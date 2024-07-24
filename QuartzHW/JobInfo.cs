using Quartz;

namespace QuartzHW
{
    public class JobInfo
    {
        public JobKey JobKey { get; set; }
        
        public TriggerKey TriggerKey { get; set; }
        
        public IJobDetail JobDetail { get; set; }
        
        public IEnumerable<ITrigger> Triggers { get; set; }
    }
}