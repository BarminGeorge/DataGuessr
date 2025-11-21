using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Interfaces;

public interface IEvaluationService
{
    Score CalculateScore(Answer answer, GameMode mode);
}