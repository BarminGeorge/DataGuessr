using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IEvaluationService
{
    Func<Answer, Answer, Score> CalculateScore(GameMode mode);
}