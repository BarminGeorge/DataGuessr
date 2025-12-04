using Domain.Enums;
using Domain.Interfaces;

namespace Domain.ValueTypes.Inputs;

public record ImageQuestionType(string ImageData) : IQuestionType
{
    public QuestionType Type => QuestionType.Image;
}