using Domain.Enums;

namespace Domain.Interfaces;

public interface IMode
{
    string Name { get; init; }
    string Description { get; init; }
    InputType SupportedInputType { get; }
}