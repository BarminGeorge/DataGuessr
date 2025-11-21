using Application.Dto;
using Domain.Enums;
using Domain.ValueTypes;

namespace Application.Requests_Responses;

public record CreateRoomResponse(bool Success, RoomDto RoomDto);
public record JoinRoomResponse(bool Success, RoomDto RoomDto);
public record LeaveRoomResponse(bool Success);

public record CreateGameResponse(GameMode mode, int countQuestions, TimeSpan QuestionDuration, IEnumerable<Question> questions = null);
public record StartGameResponse(bool Success);
public record SubmitAnswerResponse(bool Success);