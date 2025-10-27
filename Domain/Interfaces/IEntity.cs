namespace Domain.Interfaces;

internal interface IEntity<out T>
{
    T Id { get; }
}