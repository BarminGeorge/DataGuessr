using Application.DI;
using Application.EndPoints;
using Application.Interfaces;
using Application.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.DI;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

services.AddApplication();
services.AddInfrastructure(configuration);

services.AddHangfire(config => config
    .UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));

services.AddHangfireServer();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions 
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always,
});

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapGet("/api", () => "Hello World!");
app.MapUserEndpoints();
app.MapRoomEndpoints();

RecurringJob.AddOrUpdate<IGuestCleanupService>(
    "cleanup-orphaned-guests",
    service => service.CleanupOrphanedGuestsAsync(CancellationToken.None),
    Cron.Hourly);

RecurringJob.AddOrUpdate<IGuestCleanupService>(
    "cleanup-expired-rooms",
    service => service.CleanupExpiredRoomsAsync(CancellationToken.None),
    Cron.Daily);

app.Run();