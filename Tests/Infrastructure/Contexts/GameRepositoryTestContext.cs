using Domain.Entities;
using Domain.ValueTypes;
using Infrastructure.Interfaces;
using Infrastructure.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure;

public class GameRepositoryTestContext : DbContext, IDataContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<PlayerAnswer> PlayerAnswers { get; set; }
    public DbSet<Avatar> Avatars { get; set; }

    public GameRepositoryTestContext(DbContextOptions<GameRepositoryTestContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Question>();
        modelBuilder.Ignore<PlayerAnswer>();

        // ============ USER ============
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

        // ============ AVATAR ============
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

        // ============ PLAYER ============
        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("players");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.UserId).IsRequired();
            entity.Property(p => p.RoomId).IsRequired();
            entity.Property(p => p.ConnectionId).IsRequired().HasMaxLength(255);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Room>()
                .WithMany(r => r.Players)
                .HasForeignKey(p => p.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(p => new { p.UserId, p.RoomId }).IsUnique();
            entity.HasIndex(p => p.ConnectionId).IsUnique();
            entity.HasIndex(p => p.RoomId);
        });

        // ============ ROOM ============
        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("rooms");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Owner).IsRequired();
            entity.Property(r => r.Privacy).IsRequired();
            entity.Property(r => r.Status).IsRequired();
            entity.Property(r => r.MaxPlayers).IsRequired();
            entity.Property(r => r.Password).HasMaxLength(128);
            entity.Property(r => r.ClosedAt).IsRequired();
            entity.HasMany(r => r.Players)
                .WithOne()
                .HasForeignKey(p => p.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(r => r.Games)
                .WithOne()
                .HasForeignKey(g => g.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.ClosedAt);
        });

        // ============ GAME ============
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("games");
            entity.HasKey(g => g.Id);
            entity.Property(g => g.RoomId).IsRequired();
            entity.Property(g => g.Mode).IsRequired();
            entity.Property(g => g.Status).IsRequired();
            entity.Property(g => g.QuestionsCount).IsRequired();
            entity.Property(g => g.QuestionDuration).IsRequired();
            entity.Property(g => g.CurrentStatistic)
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Statistic>(v, (System.Text.Json.JsonSerializerOptions)null)
                )
                .IsRequired(false);
            entity.HasOne<Room>()
                .WithMany(r => r.Games)
                .HasForeignKey(g => g.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(g => g.Status);
            entity.HasIndex(g => g.RoomId);
        });
    }
}
