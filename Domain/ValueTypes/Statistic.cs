namespace Domain.ValueTypes;

public record Statistic
{
    public IReadOnlyDictionary<Guid, Score> Scores => scores;
    private readonly Dictionary<Guid, Score> scores = new();

    public Statistic() { }
    
    private Statistic(Dictionary<Guid, Score> scores)
    {
        this.scores = scores;
    }

    public void Update(Dictionary<Guid, Answer> answers, Answer rightAnswer, Func<Answer, Answer, Score> update)
    {
        foreach (var (id, answer) in answers)
            scores[id] = Scores.TryGetValue(id, out var current) 
                ? current + update(answer, rightAnswer) 
                : update(answer, rightAnswer);
    }
    
    public Statistic Diff(Statistic other) => this - other;
    
    public static Statistic operator -(Statistic left, Statistic right)
    {
        var diffScores = new Dictionary<Guid, Score>();

        var allKeys = left.Scores.Keys.Union(right.Scores.Keys);

        foreach (var key in allKeys)
        {
            left.Scores.TryGetValue(key, out var leftScore);
            right.Scores.TryGetValue(key, out var rightScore);

            var diff = (leftScore ?? Score.Zero) - (rightScore ?? Score.Zero);
            diffScores[key] = diff;
        }

        return new Statistic(diffScores);
    }
    
    public Statistic Copy()
    {
        var copiedScores = new Dictionary<Guid, Score>(scores);
        return new Statistic(copiedScores);
    }
}