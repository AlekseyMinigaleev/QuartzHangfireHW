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

        [HttpPost("resume/{jobGroup}/{jobName}")]
        public async Task<IActionResult> ResumeJobAsync(
            string jobGroup,
            string jobName)
        {
            var job = await GetJobDetailAsync(jobName,jobGroup);

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.ResumeJob(job.Key);
            return Ok($"Job {job.Key} Resumed.");
        }

        [HttpDelete("delete/{jobGroup}/{jobName}")]
        public async Task<IActionResult> DeleteJobAsync(
            string jobGroup,
            string jobName)
        {
            var job = await GetJobDetailAsync(jobName, jobGroup);

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.DeleteJob(job.Key);
            return Ok($"Job {job.Key} deleted.");
        }

        [HttpPost("stop/{jobGroup}/{jobName}")]
        public async Task<IActionResult> StopJobAsync(
            string jobGroup,
            string jobName)
        {
            var job = await GetJobDetailAsync(jobName, jobGroup);

            if (job is null)
                return NotFound("Job not found.");

            await _scheduler.PauseJob(job.Key);
            return Ok($"Job {job.Key} stoped.");
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
                        JobName = jobKey.Name,
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

        [HttpPost("restore/{jobGroup}/{jobName}")]
        public async Task<IActionResult> RestoreJobsAsync(
            string jobGroup,
            string jobName)
        {
            var key = new JobKey(jobName, jobGroup);
            var jobInfo = JobConfiguration.Jobs
                .SingleOrDefault(x => x.JobKey.CompareTo(key) == 0);

            if (jobInfo is null)
                return NotFound();

            await _scheduler.AddJob(jobInfo.JobDetail, true);
            foreach (var trigger in jobInfo.Triggers)
                await _scheduler.ScheduleJob(trigger);

            return Ok("job restored successfully.");
        }

        private async Task<IJobDetail?> GetJobDetailAsync(string key, string group)
        {
            var jobKey = new JobKey(key, group);
            var job = await _scheduler.GetJobDetail(jobKey);

            return job;
        }
    }
}