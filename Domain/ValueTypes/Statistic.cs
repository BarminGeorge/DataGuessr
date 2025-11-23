namespace Domain.ValueTypes;

public record Statistic
{
    public IReadOnlyDictionary<Guid, Score> Scores => scores;
    private readonly Dictionary<Guid, Score> scores = new();

    public void Update(Dictionary<Guid, Answer> question, DateTime rightAnswer, Func<Answer, DateTime, Score> update)
    {
        foreach (var (id, answer) in question)
            scores[id] = Scores.TryGetValue(id, out var current) 
                ? current + update(answer, rightAnswer) 
                : update(answer, rightAnswer);
    }
}