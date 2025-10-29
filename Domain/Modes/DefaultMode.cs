using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Modes;

public class DefaultMode : IMode
{
    public string Name { get; init; } = "Обычный режим";

    public string Description { get; init; } =
        "Тебе дается картинка, а ты должен как можно точнее отгадать время когда она была сделана";
    
    public QuestionType SupportedQuestionType { get; } = QuestionType.Image;
}