using Domain.Interfaces;
using Domain.ValueTypes;

namespace Domain.Entities;

public class Question(Answer rightAnswer, string formulation, string imageUrl) : IEntity<Guid>
{
    public Guid Id { get; } = Guid.NewGuid();
    public Answer RightAnswer { get; } = rightAnswer;
    public string Formulation { get; } = formulation;
    public string ImageUrl { get; } = imageUrl;
}