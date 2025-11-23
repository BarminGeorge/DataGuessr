using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IEvaluationService
{
    Func<Answer, DateTime, Score> CalculateScore(GameMode mode);
}