using Domain.ValueTypes;

namespace Application.Interfaces
{
    public interface IRoundService
    {
        Task<RoundDto?> GetCurrentRoundAsync(Guid roomId, Guid gameId);
        Task<AnswerResultDto> SubmitAnswerAsync(Guid roomId, Guid gameId, Guid roundId, Guid userId, Answer answer);
        Task<RoundResultsDto> GetRoundResultsAsync(Guid roomId, Guid gameId, Guid roundId);
        Task<GameLeaderboardDto> GetGameLeaderboardAsync(Guid roomId, Guid gameId);
        Task<GameStatusDto> GetGameStatusAsync(Guid roomId, Guid gameId);
    }
}

public record RoundDto(
    Guid Id,
    int RoundNumber,
    int TotalRounds,
    string ImageUrl,
    string Question,
    TimeSpan TimeLimit,
    DateTime StartTime,
    DateTime EndTime
);

public record AnswerResultDto(
    Guid RoundId,
    Guid PlayerId,
    int Score,
    bool IsCorrect,
    TimeSpan TimeRemaining
);

public record RoundResultsDto(
    Guid RoundId,
    int RoundNumber,
    int TotalRounds,
    Answer CorrectAnswer,
    List<PlayerRoundResultDto> PlayerResults,
    int? NextRoundNumber
);

public record PlayerRoundResultDto(
    Guid PlayerId,
    string PlayerName,
    Answer Answer,
    int Score,
    DateTime AnswerTime
);

public record GameStatusDto(
    Guid GameId,
    string Status,
    int CurrentRound,
    int TotalRounds,
    TimeSpan? TimeUntilNextRound,
    bool IsGameFinished
);

public record GameLeaderboardDto(
    Guid GameId,
    Guid RoomId,
    string GameTitle,
    int TotalRounds,
    int CompletedRounds,
    List<LeaderboardEntryDto> Entries,
    bool IsGameFinished
);

public record LeaderboardEntryDto(
    Guid PlayerId,
    string PlayerName,
    int TotalScore
);