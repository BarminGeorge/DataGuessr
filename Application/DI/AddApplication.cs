using Application.Interfaces;
using Application.Notifications;
using Application.Requests_Responses;
using Application.Requests_Responses.Validators;
using Application.Requests_Responses.Validators.ParameterValidators;
using Application.Services;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Application.DI;

public static class AddApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddServices();
        services.AddValidators();
        
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<IFormFile>, ImageValidator>();
        services.AddScoped<IValidator<string>, LoginValidator>();
        services.AddScoped<IValidator<string>, PasswordUserValidator>();
        services.AddScoped<IValidator<string>, PlayerNameValidator>();
        services.AddScoped<IValidator<string?>, RoomPasswordValidator>();
        services.AddScoped<IValidator<CreateGameRequest>, CreateGameRequestValidator>();
        services.AddScoped<IValidator<CreateGuestRequest>, CreateGuestRequestValidator>();
        services.AddScoped<IValidator<CreateRoomRequest>, CreateRoomRequestValidator>();
        services.AddScoped<IValidator<FindQuickRoomRequest>, FindQuickRoomRequestValidator>();
        services.AddScoped<IValidator<JoinRoomRequest>, JoinRoomRequestValidator>();
        services.AddScoped<IValidator<LeaveRoomRequest>, LeaveRoomRequestValidator>();
        services.AddScoped<IValidator<LoginUserRequest>, LoginUserRequestValidator>();
        services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestValidator>();
        services.AddScoped<IValidator<StartGameRequest>, StartGameRequestValidator>();
        services.AddScoped<IValidator<SubmitAnswerRequest>, SubmitAnswerRequestValidator>();
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();
        services.AddScoped<IValidator<KickPlayerRequest>, KickPlayerRequestValidator>();
        services.AddScoped<IValidator<FinishGameRequest>, FinishGameRequsetValidator>();

        services.AddFluentValidationAutoValidation();
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
        services.AddScoped<IRoomManager, RoomManager>();
        services.AddScoped<IGameManager, GameManager>();
        services.AddScoped<IConnectionService, ConnectionService>();
        services.AddScoped<IEvaluationService, EvaluateService>();
        services.AddScoped<IGameCoreService, GameCoreService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IGuestCleanupService, GuestCleanupService>();
        services.AddScoped<INotificationService, SignalRNotificationService>();
        
        return services;
    }
}