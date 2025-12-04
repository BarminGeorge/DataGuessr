using FluentValidation;

namespace Application.Requests_Responses.Validators;

public class KickPlayerRequestValidator : AbstractValidator<KickPlayerRequest>
{
    public KickPlayerRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
        
        RuleFor(x => x.RoomId)
            .NotEmpty().WithMessage("Room ID is required");
        
        RuleFor(x => x.RemovedPlayerId)
            .NotEmpty().WithMessage("Player to remove ID is required");
    }
}