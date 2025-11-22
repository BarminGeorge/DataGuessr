using System.Formats.Asn1;
using Application.Interfaces;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Services;

public class EvaluateService: IEvaluationService
{
    public Score CalculateScore(Answer answer, DateTime rightAnswer, GameMode mode)
    {
        var dif = Math.Abs((rightAnswer - answer.Date).TotalDays);
        return mode switch
        {
            GameMode.DefaultMode => new Score((int)Math.Round(2222 * Math.Exp(-dif / 10000))),
            _ => throw new ArgumentException($"Для {mode} подсчет очков не реализован.")
        };
    }
}