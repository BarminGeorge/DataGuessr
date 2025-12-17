using Application.Endpoints;
using Application.EndPoints;
using Application.Endpoints.Hubs;
using Application.Interfaces;
using Hangfire;
using Infrastructure.PostgreSQL;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

namespace Application;

public static class ConfigurationApp
{
    public static void ConfigureApp(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AllowAll");
        }
        else
        {
            app.UseHsts();
            app.UseCors("ProductionPolicy");
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
        app.UseAntiforgery();

        app.UseHangfireDashboard("/hangfire");

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }

        app.AddMappedEndPoints();
        app.MapControllers();

        RecurringJob.AddOrUpdate<IGuestCleanupService>(
          "cleanup-orphaned-guests",
        service => service.CleanupOrphanedGuestsAsync(CancellationToken.None),
                Cron.Hourly);

        RecurringJob.AddOrUpdate<IGuestCleanupService>(
        "cleanup-expired-rooms",
        service => service.CleanupExpiredRoomsAsync(CancellationToken.None),
                Cron.Daily);
    }
    
    private static void AddMappedEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoints();
        app.MapRoomEndpoints();
        app.MapImageEndpoints();
        
        app.MapHub<AppHub>("api/appHub");
    }
}