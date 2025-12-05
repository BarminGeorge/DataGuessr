using FluentValidation;

namespace Application.Requests.Validators;

public class FindQuickRoomRequestValidator : AbstractValidator<FindQuickRoomRequest>
{
    public FindQuickRoomRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}