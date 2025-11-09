namespace Application.Interfaces;

public interface IRepository
{
    public void Add<T>(T value);
    public void Get();
}