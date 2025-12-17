using System.Text.Json.Serialization;
using Application.DI;
using Application.Endpoints;
using Application.EndPoints;
using Application.Endpoints.Hubs;
using Domain.ValueTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Tests.Application.Integration;

public static class TestProgram
{
    public static void ConfigureApplication(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        services.AddApplication();
        services.AddFluentValidationAutoValidation();
        
        services.AddAntiforgery();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy => 
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
        });

        services.AddAuthentication(_ => { })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

        services.AddEndpointsApiExplorer();
        services.AddControllers();
        
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    public static void ConfigureApp(WebApplication app)
    {
        app.UseCors("AllowAll");
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

        app.MapUserEndpoints();
        app.MapRoomEndpoints();
        app.MapHub<AppHub>("/appHub");
        app.MapControllers();
    }
}