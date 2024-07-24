using Hangfire;
using HangfireHW;
using HangfireHW.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("HangfireDb")!;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHangfire(connectionString);

var app = builder.Build();

app.UseHangfireDashboard();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
JobConfiguration.RegisterAllJobs();
app.Run();