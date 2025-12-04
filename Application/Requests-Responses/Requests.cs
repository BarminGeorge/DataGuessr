using Domain.Entities;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Requests_Responses;

public record CreateRoomRequest(Guid UserId, RoomPrivacy Privacy, string? Password, int MaxPlayers);
public record JoinRoomRequest(Guid UserId, Guid RoomId, string? Password);
public record LeaveRoomRequest(Guid UserId, Guid RoomId);
public record FindQuickRoomRequest(Guid UserId);

public record RegisterUserRequest(string Login, string Password, string PlayerName, IFormFile Avatar);
public record LoginUserRequest(string Login, string Password);
public record UpdateUserRequest(Guid UserId, IFormFile Avatar, string PlayerName);
public record CreateGuestRequest(string PlayerName, IFormFile Avatar);

public record CreateGameRequest(
    Guid UserId, 
    Guid RoomId, 
    GameMode Mode, 
    int CountQuestions, 
    TimeSpan QuestionDuration, 
    IEnumerable<Question>? Questions = null);

public record StartGameRequest(Guid UserId, Guid RoomId);
public record FinishGameRequest(Guid UserId, Guid RoomId);
public record SubmitAnswerRequest(Guid GameId, Guid QuestionId, Guid PlayerId, Answer Answer);
public record KickPlayerRequest(Guid UserId, Guid RoomId, Guid RemovedPlayerId);