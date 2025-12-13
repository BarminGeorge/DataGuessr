using FluentValidation;

namespace Application.Requests.Validators;

public class CreateGameRequestValidator : AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator()
    {
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("Room ID is required");

        RuleFor(x => x.Mode)
            .IsInEnum().WithMessage("Invalid game mode");

        RuleFor(x => x.CountQuestions)
            .InclusiveBetween(1, 100).WithMessage("Number of questions must be between 1 and 100");

        RuleFor(x => x.QuestionDuration)
            .GreaterThan(TimeSpan.Zero).WithMessage("Question duration must be positive")
            .LessThanOrEqualTo(TimeSpan.FromMinutes(5)).WithMessage("Question duration cannot exceed 5 minutes");
    }
}