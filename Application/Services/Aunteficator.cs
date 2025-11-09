using Application.Interface;
using Domain.Entities;

namespace Application.Service;

public class Aunteficator(IRepository repository)
{
    public void Aunteficate(string name, string avatar)
    {
        var user = new User(name, avatar);
        repository.Add(user);
    }
}