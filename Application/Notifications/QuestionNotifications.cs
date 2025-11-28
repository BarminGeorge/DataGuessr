namespace Application.Notifications;

// TODO: прописать frontend-backend сущности для передачи (нужны здесь в аргументах),
// для конвертации написать мапперы
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