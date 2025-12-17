namespace Domain.ValueTypes;

public record Score(double Points)
{
    public static Score Zero => new(0);
    public static Score operator +(Score a, Score b) => new(a.Points + b.Points);
    public static Score operator -(Score a, Score b) => new(a.Points - b.Points);
}