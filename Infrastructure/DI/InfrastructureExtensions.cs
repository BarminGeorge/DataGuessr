using Infrastructure.Interfaces;
using Infrastructure.PostgreSQL;
using Infrastructure.PostgreSQL.Repositories;
using Infrastructure.Providers;
using Infrastructure.ROMRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IDataContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPlayerAnswerRepository, PlayerAnswerRepository>();
        
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        return services;
    }
}
