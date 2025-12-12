using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueTypes;
using Newtonsoft.Json;

namespace Domain.Entities;

public class Question : IEntity<Guid>
{
    public Guid Id { get; init; }
    public Answer RightAnswer { get; init; }
    public string Formulation { get; init; }
    public string ImageUrl { get; init; }
    public GameMode Mode { get; set; }

    public virtual ICollection<Game> Games { get; private set; } = new List<Game>();
    
    public Question() { }

    [JsonConstructor]
    public Question(Answer rightAnswer, string formulation, string imageUrl, GameMode gameMode)
    {
        Id = Guid.NewGuid();
        RightAnswer = rightAnswer;
        Formulation = formulation;
        ImageUrl = imageUrl;
        Mode = gameMode;
    }
}
