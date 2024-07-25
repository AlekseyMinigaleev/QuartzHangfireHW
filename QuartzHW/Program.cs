using Quartz;
using QuartzHW.Extensions;
using QuartzHW;
using CrystalQuartz.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder
    .Configuration
    .GetConnectionString("QuartzDB")!;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddQuartz(connectionString);

var app = builder.Build();

await CreateSchedulerAsync(
    app.Services.GetRequiredService<ISchedulerFactory>());
var scheduler = app
    .Services
    .GetRequiredService<ISchedulerFactory>()
    .GetScheduler();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCrystalQuartz(() => scheduler);
app.MapControllers();
app.Run();

async Task<IScheduler> CreateSchedulerAsync(ISchedulerFactory schedulerFactory)
{
    var scheduler = schedulerFactory.GetScheduler().Result;

    JobConfiguration.Jobs
        .ForEach(async x => await scheduler
            .ScheduleJob(
                x.JobDetail,
                x.Triggers.ToList(),
                false)
        );

    await scheduler.Start();

    return scheduler;
}