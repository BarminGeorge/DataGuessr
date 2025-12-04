using System.Text;
using Application.EndPoints;
using Infrastructure.ValueTypes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Extensions;

public static class ApiExtensions
{
    public static void AddMappedEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoints();
    }

    public static void AddApiAuthentication(this IServiceCollection services, IOptions<JwtOptions> jwtoptions)
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
                        context.Token = context.Request.Cookies[""];
                        return Task.CompletedTask;
                    }
                };
            });
        
        services.AddAuthorization();
    }
}