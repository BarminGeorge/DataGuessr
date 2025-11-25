using Domain.Entities;
using Domain.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Avatar> Avatars { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne<Avatar>()
                .WithMany()
                .HasForeignKey(u => u.AvatarId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(u => u.Login).IsUnique();
        });

        // Конфигурация Player
        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("players");
            entity.HasKey(p => p.Id);

            // Связь с User
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь с Room
            entity.HasOne<Room>()
                .WithMany()
                .HasForeignKey(p => p.RoomId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(p => p.GuestName)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.HasOne<Avatar>()
                .WithMany()
                .HasForeignKey(p => p.GuestAvatarId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Property(p => p.Score)
                .HasConversion(
                    score => score.score,
                    value => new Score(value) 
                )
                .IsRequired();

            // Check constraint: либо UserId, либо GuestName должен быть заполнен
            entity.HasCheckConstraint(
                "CK_Player_UserOrGuest",
                @"(user_id IS NOT NULL AND guest_name IS NULL) OR 
              (user_id IS NULL AND guest_name IS NOT NULL)"
            );

            entity.HasIndex(p => p.UserId);
            entity.HasIndex(p => p.RoomId);
            entity.HasIndex(p => new { p.UserId, p.RoomId }).IsUnique();
        });
    }
}