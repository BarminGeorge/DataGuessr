using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure;

public class UserRepositoryTestContext : DbContext, IDataContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<PlayerAnswer> PlayerAnswers { get; set; }
    public DbSet<Avatar> Avatars { get; set; }

    public UserRepositoryTestContext(DbContextOptions<UserRepositoryTestContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Game>();
        modelBuilder.Ignore<Room>();
        modelBuilder.Ignore<Question>();
        modelBuilder.Ignore<PlayerAnswer>();
        modelBuilder.Ignore<Player>();

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.PlayerName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Login).HasMaxLength(50);
            entity.Property(u => u.PasswordHash).HasMaxLength(255);
            entity.HasIndex(u => u.Login).IsUnique();
            entity.HasIndex(u => u.PlayerName);
        });

        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.ToTable("avatars");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.UserId).IsRequired();
            entity.Property(a => a.Filename).IsRequired().HasMaxLength(255);
            entity.Property(a => a.Mimetype).IsRequired().HasMaxLength(50);
            entity.HasOne(a => a.User)
                .WithOne(u => u.Avatar)
                .HasForeignKey<Avatar>(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(a => a.UserId).IsUnique();
        });
    }
}
