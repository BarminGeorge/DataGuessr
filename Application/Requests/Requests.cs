using Application.DtoUI;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Application.Requests;

public record CreateRoomRequest(Guid UserId, RoomPrivacy Privacy, string? Password, int MaxPlayers);
public record JoinRoomRequest(Guid UserId, string InviteCode, string? Password);
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
    int QuestionDuration);

public record StartGameRequest(Guid UserId, Guid RoomId);
public record FinishGameRequest(Guid UserId, Guid RoomId);
public record SubmitAnswerRequest(Guid GameId, Guid QuestionId, Guid PlayerId, AnswerDto Answer);
public record KickPlayerRequest(Guid UserId, Guid RoomId, Guid RemovedPlayerId);
public record GetRoomPrivacyRequest([FromRoute] string InviteCode);