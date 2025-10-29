using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Modes;

public class DefaultMode : IMode
{
    public QuestionType SupportedQuestionType { get; } = QuestionType.Image;
}