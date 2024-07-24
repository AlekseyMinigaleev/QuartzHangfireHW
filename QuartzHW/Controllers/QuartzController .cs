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
        private readonly IScheduler _scheduler;

        public QuartzController(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            _scheduler = _schedulerFactory
                .GetScheduler().Result;
        }

        [HttpPost("resume/{jobKey}")]
        public async Task<IActionResult> ResumeJob(string jobKey)
        {
            var job = await _scheduler.GetJobDetail(new JobKey(jobKey));

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.ResumeJob(new JobKey(jobKey));
            return Ok($"Job {jobKey} Resumed.");
        }

        [HttpDelete("delete/{jobKey}")]
        public async Task<IActionResult> DeleteJob(string jobKey)
        {
            var job = await _scheduler.GetJobDetail(new JobKey(jobKey));

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.DeleteJob(new JobKey(jobKey));
            return Ok($"Job {jobKey} deleted.");
        }

        [HttpPost("stop/{jobKey}")]
        public async Task<IActionResult> StopJob(string jobKey)
        {
            var job = await _scheduler.GetJobDetail(new JobKey(jobKey));

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.PauseJob(new JobKey(jobKey));
            return Ok($"Job {jobKey} deleted.");
        }

        [HttpGet("jobs")]
        public async Task<IActionResult> GetJobs()
        {
            var jobGroups = await _scheduler.GetJobGroupNames();
            var jobs = new List<object>();

            foreach (var group in jobGroups)
            {
                var jobKeys = await _scheduler
                    .GetJobKeys(GroupMatcher<JobKey>
                        .GroupEquals(group));

                foreach (var jobKey in jobKeys)
                {
                    var jobDetail = await _scheduler.GetJobDetail(jobKey);
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

        //[HttpPost("restore")]
        //public IActionResult RestoreJobs([FromServices] IServiceProvider serviceProvider)
        //{
        //    var quartzConfigurator = serviceProvider
        //        .GetRequiredService<IServiceCollectionQuartzConfigurator>();

        //    ServiceCollectionQuartzConfiguratorExtensions.AddJobs(quartzConfigurator);
        //    ServiceCollectionQuartzConfiguratorExtensions.AddTriggers(quartzConfigurator);

        //    return Ok("All jobs restored successfully.");
        //}
    }
}