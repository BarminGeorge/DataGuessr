using Domain.Entities;
using Domain.Interfaces;
using Domain.Modes;

namespace Domain.Builders;

public static class GameBuilder
{
    public static GameModeSelector Create() => new();
}

public class GameModeSelector
{
    public Game WithMode<TMode>(TMode mode) where TMode : IMode 
        => new(mode);
    
    public Game WithDefaultMode() => new(new DefaultMode());
}