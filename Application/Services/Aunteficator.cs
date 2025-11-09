using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class Aunteficator(IRepository repository)
{
    public void Aunteficate(string name, string avatar)
    {
        var user = new User(name, avatar);
        repository.Add(user);
    }
}