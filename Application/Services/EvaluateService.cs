using Application.Interfaces;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Services;

public class EvaluateService: IEvaluationService
{
    public Func<Answer, DateTime, Score> CalculateScore(GameMode mode)
    {
        return mode switch
        {
            GameMode.DefaultMode => 
                (answer, rightAnswer)  => 
                    new Score((int)Math.Round(2222 * Math.Exp(-1 * (Math.Abs((rightAnswer - answer.Date).TotalDays)) / 10000))),
            _ => throw new ArgumentException($"Для {mode} подсчет очков не реализован.")
        };
    }
}