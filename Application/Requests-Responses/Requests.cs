using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Requests_Responses;

public record CreateRoomRequest(Guid UserId, [Required] RoomPrivacy Privacy, string? Password, int MaxPlayers);
public record JoinRoomRequest(Guid UserId, Guid RoomId, string? Password);
public record LeaveRoomRequest(Guid UserId, Guid RoomId);
public record FindQuickRoomRequest(Guid UserId);

public record RegisterUserRequest([Required] string Username, [Required] string Password);
public record LoginUserRequest([Required] string Username, [Required] string Password);

public record CreateGameRequest(
    Guid UserId, 
    Guid RoomId, 
    GameMode Mode, 
    int CountQuestions, 
    TimeSpan QuestionDuration, 
    IEnumerable<Question>? Questions = null);

public record StartGameRequest(Guid UserId, Guid RoomId);
public record SubmitAnswerRequest(Guid RoomId, Answer Answer);