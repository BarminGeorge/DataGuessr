namespace Application.Notifications;

public record NewQuestionNotification(Guid QuestionId, string Formulation, string ImageUrl, DateTime EndTime, int DurationSeconds)
    : GameNotification
{
    public override string MethodName => "QuestionWasAsked";
}

public record QuestionClosedNotification(Guid QuestionId, DateTime CorrectAnswer)
    : GameNotification
{
    public override string MethodName => "QuestionClosed";
}