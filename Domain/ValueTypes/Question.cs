using Domain.Interfaces;

namespace Domain.ValueTypes;

public record Question
{
    public IInput Input { get; }
    public Answer RightAnswer { get; }
    public IReadOnlyList<Answer> AllAnswers => allAnswers.AsReadOnly();

    private readonly List<Answer> allAnswers = [];

    public Question(IInput input, Answer rightAnswer)
    {
        Input = input;
        RightAnswer = rightAnswer;
        allAnswers.Add(rightAnswer);
    }

    public void AddAnswer(Answer answer) => allAnswers.Add(answer);
}