using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Modes;

public class DefaultMode : IMode
{
    public GameMode GameMode { get; } = GameMode.DefaultMode;
    public QuestionType SupportedQuestionType { get; } = QuestionType.Image;
}