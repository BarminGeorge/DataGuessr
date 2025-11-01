namespace Domain.ValueTypes;

public record Statistic
{
    public IReadOnlyDictionary<Guid, Score> Scores => Scores;
    private readonly Dictionary<Guid, Score> scores = new();

    public void Update(Question question, Func<Answer, Score> update)
    {
        foreach (var answer in question.AllAnswers)
            scores[answer.PlayerId] = Scores.TryGetValue(answer.PlayerId, out var current) 
                ? current + update(answer) 
                : update(answer);
    }
}