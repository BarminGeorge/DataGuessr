using System.Text.Json.Serialization;
using Application.DI;
using Application.Endpoints;
using Application.EndPoints;
using Application.Interfaces;
using Application.Endpoints.Hubs;
using Domain.ValueTypes;
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

services.AddAntiforgery();

//services.AddHangfire(config => config.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));

//services.AddHangfireServer();

services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
        policy.WithOrigins("https://fiitguesser.ru").AllowAnyHeader().AllowAnyMethod());
});


services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});

services.AddAuthentication(_ =>
    {
    
    })
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.UseInlineDefinitionsForEnums();
});
services.AddControllers();
services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseCors("ProductionPolicy");

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions 
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always,
});

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

//app.UseHangfireDashboard("/hangfire");

app.MapUserEndpoints();
app.MapRoomEndpoints();
app.MapImageEndpoints();
app.MapHub<AppHub>("/appHub");

app.MapControllers();

//RecurringJob.AddOrUpdate<IGuestCleanupService>(
   //"cleanup-orphaned-guests",
    //service => service.CleanupOrphanedGuestsAsync(CancellationToken.None),
    //Cron.Hourly);

//RecurringJob.AddOrUpdate<IGuestCleanupService>(
    //"cleanup-expired-rooms",
    //service => service.CleanupExpiredRoomsAsync(CancellationToken.None),
    //Cron.Daily);

app.Run();