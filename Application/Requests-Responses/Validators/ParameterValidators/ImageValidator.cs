using Application.Extensions;
using FluentValidation;

namespace Application.Requests_Responses.Validators.ParameterValidators;

public class ImageValidator : AbstractValidator<IFormFile>
{
    public ImageValidator()
    {
        RuleFor(x => x)
            .Must(x => x.IsValid()).WithMessage("Invalid image file")
            .When(x => x != null);
    }
}