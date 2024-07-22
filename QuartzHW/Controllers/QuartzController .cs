using Microsoft.AspNetCore.Mvc;
using Quartz.Impl.Matchers;
using Quartz;

namespace QuartzHW.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuartzController : ControllerBase
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public QuartzController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }

        [HttpPost("start/{jobKey}")]
        public async Task<IActionResult> StartJob(string jobKey)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = await scheduler.GetJobDetail(new JobKey(jobKey));

            if (job is null)
                return NotFound("Job not found.");

            await scheduler.TriggerJob(new JobKey(jobKey));
            return Ok($"Job {jobKey} triggered.");
        }

        [HttpPost("stop/{jobKey}")]
        public async Task<IActionResult> StopJob(string jobKey)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var job = await scheduler.GetJobDetail(new JobKey(jobKey));

            if (job is null)
                return NotFound("Job not found.");

            await scheduler.DeleteJob(new JobKey(jobKey));
            return Ok($"Job {jobKey} deleted.");
        }

        [HttpGet("jobs")]
        public async Task<IActionResult> GetJobs()
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobGroups = await scheduler.GetJobGroupNames();
            var jobs = new List<object>();

            foreach (var group in jobGroups)
            {
                var jobKeys = await scheduler
                    .GetJobKeys(GroupMatcher<JobKey>
                        .GroupEquals(group));

                foreach (var jobKey in jobKeys)
                {
                    var jobDetail = await scheduler.GetJobDetail(jobKey);
                    jobs.Add(new
                    {
                        jobKey = jobKey.Name,
                        jobGroup = jobKey.Group,
                        jobClass = jobDetail.JobType.FullName,
                    });
                }
            }

            return Ok(jobs);
        }
    }
}
