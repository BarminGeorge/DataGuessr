using Domain.Interfaces;

namespace Domain.ValueTypes;

public record Question
{
    public IQuestionType QuestionType { get; }
    public Answer RightAnswer { get; }
    public IReadOnlyList<Answer> AllAnswers => allAnswers.AsReadOnly();

    private readonly List<Answer> allAnswers = [];

    public Question(IQuestionType questionType, Answer rightAnswer)
    {
        QuestionType = questionType;
        RightAnswer = rightAnswer;
    }

    public void AddAnswer(Answer answer) => allAnswers.Add(answer);
}