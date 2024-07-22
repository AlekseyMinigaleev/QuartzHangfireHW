using Quartz;
using Quartz.AspNetCore;
using QuartzHW.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(opt =>
    {
        opt.UsePostgres(builder.Configuration.GetConnectionString("DefaultConnection"));
        opt.UseNewtonsoftJsonSerializer();
    });

    var jobKey = new JobKey("WriteHelloWorld");
    q.AddJob<WriteHelloWorldJob>(opt => opt.WithIdentity(jobKey));

    q.AddTrigger(opt => opt
        .ForJob(jobKey)
        .WithIdentity($"{jobKey}-trigger")
        .WithSimpleSchedule(x => x
            .WithInterval(TimeSpan.FromMilliseconds(100))
            .RepeatForever())
        );
});
builder.Services.AddQuartzServer(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();