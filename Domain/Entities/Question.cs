using Domain.Interfaces;

namespace Domain.ValueTypes;

//TODO update 
public class Question : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public IQuestionType QuestionType { get; }
    public Answer RightAnswer { get; private set; }
    public IReadOnlyList<Answer> AllAnswers => allAnswers.AsReadOnly();

    private readonly List<Answer> allAnswers = [];

    protected Question()
    {
    }

    public Question(IQuestionType questionType, Answer rightAnswer)
    {
        QuestionType = questionType;
        RightAnswer = rightAnswer;
    }

    public void AddAnswer(Answer answer) => allAnswers.Add(answer);
}