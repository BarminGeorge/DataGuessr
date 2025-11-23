using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Room> Rooms { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связи User -> Player (один к одному)
        modelBuilder.Entity<Player>()
            .HasOne(p => p.User)
            .WithOne() 
            .HasForeignKey<Player>(p => p.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}