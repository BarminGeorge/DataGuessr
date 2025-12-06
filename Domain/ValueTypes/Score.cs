namespace Domain.ValueTypes;

public record Score(double score)
{
    public static Score Zero => new(0);
    public static Score operator +(Score a, Score b) => new(a.score + b.score);
    public static Score operator -(Score a, Score b) => new(a.score - b.score);
}