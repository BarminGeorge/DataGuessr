using Application.Interfaces;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Services;

public class EvaluateService: IEvaluationService
{
    public Func<Answer, Answer, Score> CalculateScore(GameMode mode)
    {
        return mode switch
        {
            GameMode.DefaultMode => (a1, a2) =>
            {
                if (a1 is DateTimeAnswer d1 && a2 is DateTimeAnswer d2)
                    return new Score(
                        (int)Math.Round(2222 * Math.Exp(-1 * (Math.Abs((d1.Value - d2.Value).TotalDays)) / 10000)));
                throw new ArgumentException("Wrong answer type");
            },
        
            GameMode.BoolMode => (a1, a2) =>
            {
                if (a1 is BoolAnswer b1 && a2 is BoolAnswer b2)
                    return new Score(((b1.Value == b2.Value) ? 1 : 0) * 20);
                throw new ArgumentException("Wrong answer type");
            },
        
            _ => throw new ArgumentException($"Unknown mode: {mode}")
        };
    }
}

