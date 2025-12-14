using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL;

public class AppDbContext : DbContext, IDataContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Avatar> Avatars { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<PlayerAnswer> PlayerAnswers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ============ USER ============
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.PlayerName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Login)
                .HasMaxLength(50);

            entity.Property(u => u.PasswordHash)
                .HasMaxLength(255);

            entity.HasIndex(u => u.Login)
                .IsUnique()
                .HasFilter("\"Login\" IS NOT NULL");

            entity.HasIndex(u => u.PlayerName);
        });

        // ============ AVATAR ============
        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.ToTable("avatars");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.UserId).IsRequired();
            entity.Property(a => a.Filename)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(a => a.Mimetype)
                .IsRequired()
                .HasMaxLength(50);

            // 1:1 связь
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
            entity.Property(p => p.ConnectionId)
                .IsRequired()
                .HasMaxLength(255);

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

        // ============ QUESTION ============
        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("questions");
            entity.HasKey(q => q.Id);

            entity.Property(q => q.Formulation)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(q => q.RightAnswer)
                .HasColumnType("jsonb")
                .IsRequired();

            entity.Property(q => q.Mode)
                .IsRequired()
                .HasConversion<int>();

            entity.HasIndex(q => q.Mode);

            entity.HasMany(q => q.Games)
                .WithMany(g => g.Questions)
                .UsingEntity<Dictionary<string, object>>(
                    "game_questions",
                    j => j
                        .HasOne<Game>()
                        .WithMany()
                        .HasForeignKey("game_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Question>()
                        .WithMany()
                        .HasForeignKey("question_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("game_id", "question_id");
                        j.ToTable("game_questions");
                    }
                );
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

            // CurrentStatistic как JSON (EF сам сериализует/десериализует)
            entity.Property(g => g.CurrentStatistic)
                .HasColumnType("jsonb")
                .IsRequired(false);

            entity.HasOne<Room>()
                .WithMany(r => r.Games)
                .HasForeignKey(g => g.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(g => g.Questions)
                .WithMany(q => q.Games)
                .UsingEntity<Dictionary<string, object>>(
                    "game_questions",
                    j => j
                        .HasOne<Question>()
                        .WithMany()
                        .HasForeignKey("question_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Game>()
                        .WithMany()
                        .HasForeignKey("game_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("game_id", "question_id");
                        j.ToTable("game_questions");
                    }
                );

            entity.HasMany<PlayerAnswer>()
                .WithOne()
                .HasForeignKey(pa => pa.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(g => g.Status);
            entity.HasIndex(g => g.RoomId);
        });

        // ============ PLAYER_ANSWER ============
        modelBuilder.Entity<PlayerAnswer>(entity =>
        {
            entity.ToTable("player_answers");
            entity.HasKey(pa => pa.Id);

            entity.Property(pa => pa.GameId).IsRequired();
            entity.Property(pa => pa.PlayerId).IsRequired();
            entity.Property(pa => pa.QuestionId).IsRequired();

            entity.Property(pa => pa.Answer)
                .HasColumnType("jsonb")
                .IsRequired();

            // Один ответ на вопрос от одного игрока в игре
            entity.HasIndex(pa => new { pa.GameId, pa.QuestionId, pa.PlayerId }).IsUnique();

            entity.HasIndex(pa => pa.GameId);
            entity.HasIndex(pa => pa.QuestionId);
            entity.HasIndex(pa => pa.PlayerId);
        });
    }
}
