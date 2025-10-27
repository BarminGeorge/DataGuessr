using Domain.Enums;
using Domain.Interfaces;

namespace Domain.ValueTypes.Inputs;

public record ImageInput : IInput
{
    public InputType Type => InputType.Image;
    public byte[] ImageData { get; }
    
    public ImageInput(byte[] imageData)
    {
        ImageData = imageData;
    }
}