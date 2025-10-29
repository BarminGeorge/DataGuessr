using Domain.Enums;

namespace Domain.Interfaces;

public interface IMode
{
    QuestionType SupportedQuestionType { get; }
}