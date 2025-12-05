using Application.Requests.Validators.ParameterValidators;
using Domain.Enums;
using FluentValidation;

namespace Application.Requests.Validators;

public class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
{
    public CreateRoomRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Privacy)
            .IsInEnum().WithMessage("Invalid privacy setting");

        RuleFor(x => x.Password)
            .SetValidator(new RoomPasswordValidator());

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required for private rooms")
            .When(x => x.Privacy == RoomPrivacy.Private);

        RuleFor(x => x.MaxPlayers)
            .InclusiveBetween(4, 50).WithMessage("Max players must be between 4 and 50");
    }
}