using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;

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
        public async Task<IActionResult> ResumeJobAsync(string jobKey)
        {
            var job = await _scheduler.GetJobDetail(new JobKey(jobKey));

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.ResumeJob(new JobKey(jobKey));
            return Ok($"Job {jobKey} Resumed.");
        }

        [HttpDelete("delete/{jobKey}")]
        public async Task<IActionResult> DeleteJobAsync(string jobKey)
        {
            var job = await _scheduler.GetJobDetail(new JobKey(jobKey));

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.DeleteJob(new JobKey(jobKey));
            return Ok($"Job {jobKey} deleted.");
        }

        [HttpPost("stop/{jobKey}")]
        public async Task<IActionResult> StopJobAsync(string jobKey)
        {
            var job = await _scheduler.GetJobDetail(new JobKey(jobKey));

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.PauseJob(new JobKey(jobKey));
            return Ok($"Job {jobKey} deleted.");
        }

        [HttpGet("get-scheduled-jobs")]
        public async Task<IActionResult> GetScheduledJobsAsync()
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

        [HttpGet("get-configured-jobs")]
        public IActionResult GetConfiguredJobs() =>
            Ok(JobConfiguration.Jobs.Select(x=>x.JobKey));

        [HttpPost("restore/{jobKey}")]
        public async Task<IActionResult> RestoreJobsAsync(
            string jobName)
        {
            var jobInfo = JobConfiguration.Jobs
                .SingleOrDefault(x => x.JobKey.Name.Equals(jobName));

            if (jobInfo is null)
                return NotFound();

            await _scheduler.AddJob(jobInfo.JobDetail, true);
            foreach (var trigger in jobInfo.Triggers)
                await _scheduler.ScheduleJob(trigger);

            return Ok("job restored successfully.");
        }
    }
}