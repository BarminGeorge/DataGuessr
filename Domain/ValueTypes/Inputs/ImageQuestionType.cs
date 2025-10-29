using Domain.Enums;
using Domain.Interfaces;

namespace Domain.ValueTypes.Inputs;

public record ImageQuestionType : IQuestionType
{
    public QuestionType Type => QuestionType.Image;
    public string ImageData { get; }
    
    public ImageQuestionType(string imageData)
    {
        ImageData = imageData;
    }
}