using System.Text;
using System.Text.Json.Serialization;
using Application.DI;
using Domain.ValueTypes;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.DI;
using Infrastructure.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application;

public static class ConfigurationBuilder
{
    public static void ConfigureBuilder(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.ConfigureJwt(configuration, builder.Environment);
        
        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddAntiforgery();

        services.AddHangfire(config => config
            .UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));

        services.AddHangfireServer();

        services.ConfigureCors(configuration);
        
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
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
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly("Infrastructure")
                ));
    }
    
    private static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var jwtSection = configuration.GetSection("JwtOptions");
        var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
        
        if (environment.IsProduction() && string.IsNullOrEmpty(jwtOptions.SecretKey))
            throw new InvalidOperationException("Установите переменную окружения: JwtOptions__SecretKey\n");
        if (string.IsNullOrEmpty(jwtOptions.SecretKey))
            jwtOptions.SecretKey = GenerateTemporaryKey();

        services.Configure<JwtOptions>(options =>
        {
            options.SecretKey = jwtOptions.SecretKey;
            options.ExpireInHours = jwtOptions.ExpireInHours;
        });

        var jwtOptionsForAuth = new JwtOptions
        {
            SecretKey = jwtOptions.SecretKey,
            ExpireInHours = jwtOptions.ExpireInHours
        };
        
        services.AddApiAuthentication(Options.Create(jwtOptionsForAuth));
        
        services.AddAuthentication()
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Name = "token";
            });
    }

    private static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy("ProductionPolicy", policy =>
                policy.WithOrigins(allowedOrigins ?? []).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            
            options.AddPolicy("AllowAll", policy => 
                policy.WithOrigins(allowedOrigins ?? []).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
        });
    }
    
    private static string GenerateTemporaryKey()
    {
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static void AddApiAuthentication(this IServiceCollection services, IOptions<JwtOptions> jwtoptions)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtoptions.Value.SecretKey)),
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["token"];
                        return Task.CompletedTask;
                    }
                };
            });
    }
}