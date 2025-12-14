using Application.Endpoints;
using Application.EndPoints;
using Application.Endpoints.Hubs;
using Microsoft.AspNetCore.CookiePolicy;

namespace Application;

public static class ConfigurationApp
{
    public static void ConfigureApp(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("ProductionPolicy");
        }
        else
        {
            app.UseHsts();
            app.UseCors("AllowAll");
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

//app.UseHangfireDashboard("/hangfire");

        app.AddMappedEndPoints();
        app.MapControllers();

//RecurringJob.AddOrUpdate<IGuestCleanupService>(
        //"cleanup-orphaned-guests",
        //service => service.CleanupOrphanedGuestsAsync(CancellationToken.None),
        //Cron.Hourly);

//RecurringJob.AddOrUpdate<IGuestCleanupService>(
        //"cleanup-expired-rooms",
        //service => service.CleanupExpiredRoomsAsync(CancellationToken.None),
        //Cron.Daily);
    }
    
    private static void AddMappedEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoints();
        app.MapRoomEndpoints();
        app.MapImageEndpoints();
        
        app.MapHub<AppHub>("/appHub");
    }
}