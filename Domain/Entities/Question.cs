using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Question : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Answer RightAnswer { get; private set; }
    public string Formulation { get; private set; }
    public string ImageUrl { get; private set; }
    public GameMode Mode { get; set; }

    public virtual ICollection<Game> Games { get; private set; } = new List<Game>();

    protected Question() { }

    public Question(Answer rightAnswer, string formulation, string imageUrl, GameMode gameMode)
    {
        Id = Guid.NewGuid();
        RightAnswer = rightAnswer;
        Formulation = formulation;
        ImageUrl = imageUrl;
        Mode = gameMode;
    }
}
