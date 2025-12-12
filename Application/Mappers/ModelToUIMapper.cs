using System.Collections.ObjectModel;
using Application.DtoUI;
using Application.Extensions;
using Domain.Entities;

namespace Application.Mappers;

public static class ModelToUiMapper
{
    public static RoomDto ToDto(this Room room)
    {
        return new RoomDto(
            room.Id, 
            room.Owner, 
            room.Players.Select(x => x.ToDto()),
            room.ClosedAt);
    }

    public static GameDto ToDto(this Game game)
    {
        return new GameDto(
            game.Id,
            game.Mode,
            game.Status,
            new ReadOnlyCollection<QuestionDto>(
                game.Questions.Select(q => q.ToDto())
                              .ToList()),
            game.QuestionsCount,
            game.QuestionDuration);
    }

    public static PlayerDto ToDto(this User user)
    {
        return new PlayerDto(
            user.Id,
            user.PlayerName,
            user.Avatar.Filename);
    }

    public static QuestionDto ToDto(this Question question)
    {
        return new QuestionDto(
            question.Mode,
            question.Formulation,
            question.ImageUrl);
    }

    public static PlayerDto ToDto(this Player player)
    {
        return new PlayerDto(
            player.Id,
            player.Name,
            player.Avatar.GetUrl());
    }
}