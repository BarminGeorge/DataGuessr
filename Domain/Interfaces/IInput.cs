using Domain.Enums;

namespace Domain.Interfaces;

public interface IInput
{
    InputType Type { get; }
}