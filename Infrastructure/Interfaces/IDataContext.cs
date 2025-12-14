using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Interfaces;

public interface IDataContext: IDisposable
{
    DbSet<User> Users { get; set; }
    DbSet<Room> Rooms { get; set; }
    DbSet<Player> Players { get; set; }
    DbSet<Game> Games { get; set; }
    DbSet<Question> Questions { get; set; }
    DbSet<PlayerAnswer> PlayerAnswers { get; set; }
    DbSet<Avatar> Avatars { get; set; }

    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
