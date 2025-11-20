using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.ValueTypes;

namespace Application.Requests;

// TODO: прописать все реквесты и ответы, можно использовать те же маперы что и для уведомлений, дто убрать
public record GameCreateRequest(IMode Mode);
public record CreateRoomRequest([Required] RoomPrivacy privacy, string? password, int maxPlayers);
public record JoinRoomRequest(string? password);
public record SubmitAnswerRequest(Answer Answer);
public record RegisterUserRequest([Required] string Username, [Required] string Password);
public record LoginUserRequest([Required] string Username, [Required] string Password);

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
