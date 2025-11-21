using Application.Dto;
using Domain.ValueTypes;

namespace Application.Interfaces
{
    public interface IQuestionService
    {
        Task<RoundDto?> GetCurrentQuestionAsync(Guid roomId, Guid gameId);
        Task<AnswerResultDto> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid roundId, Guid userId, Answer answer);
        Task<RoundResultsDto> GetQuestionResultsAsync(Guid roomId, Guid gameId, Guid roundId);
        Task<GameLeaderboardDto> GetGameLeaderboardAsync(Guid roomId, Guid gameId);
        Task<GameStatusDto> GetGameStatusAsync(Guid roomId, Guid gameId);
    }
}