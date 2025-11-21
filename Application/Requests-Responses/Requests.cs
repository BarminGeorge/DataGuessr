using System.ComponentModel.DataAnnotations;
using Application.Interfaces;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Requests_Responses;

public record CreateRoomRequest(Guid userId, [Required] RoomPrivacy privacy, string? password, int maxPlayers);
public record JoinRoomRequest(Guid userId, Guid roomId, string? password);
public record LeaveRoomRequest(Guid userId, Guid roomId);
public record FindQuickRoomRequest(Guid userId);

public record RegisterUserRequest([Required] string Username, [Required] string Password);
public record LoginUserRequest([Required] string Username, [Required] string Password);

public record CreateGameRequest(
    Guid userId, 
    Guid roomId, 
    GameMode mode, 
    int countQuestions, 
    TimeSpan QuestionDuration, 
    IEnumerable<Question> questions = null);

public record StartGameRequest(Guid userId, Guid roomId);
public record SubmitAnswerRequest(Guid roomId, Answer Answer);