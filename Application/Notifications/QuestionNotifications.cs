using Application.DtoUI;
using Domain.ValueTypes;

namespace Application.Notifications;

public record NewQuestionNotification(QuestionDto Question)
    : GameNotification
{
    public override string MethodName => "QuestionWasAsked";
}

public record QuestionClosedNotification(AnswerDto CorrectAnswer)
    : GameNotification
{
    public override string MethodName => "QuestionClosed";
}