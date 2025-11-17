using Domain.Enums;

namespace Domain.Interfaces;

public interface IMode
{
    GameMode GameMode { get; }
    QuestionType SupportedQuestionType { get; }
}