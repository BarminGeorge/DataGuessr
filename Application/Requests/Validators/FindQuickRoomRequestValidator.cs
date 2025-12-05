using FluentValidation;

namespace Application.Requests_Responses.Validators;

public class FindQuickRoomRequestValidator : AbstractValidator<FindQuickRoomRequest>
{
    public FindQuickRoomRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}