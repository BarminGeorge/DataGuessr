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
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Login).HasMaxLength(50);
            entity.Property(u => u.PasswordHash).HasMaxLength(255);

            entity.HasOne<Avatar>()
                .WithMany()
                .HasForeignKey(u => u.AvatarId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(u => u.Login).IsUnique().HasFilter(null); // если Nullable<Login>
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("players");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Score)
                .HasConversion(
                    score => score.score,
                    value => new Score(value)
                )
                .IsRequired();

            entity.Property(p => p.RoomId)
                .IsRequired();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => new { p.UserId, p.RoomId }).IsUnique();

            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("rooms");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Code).HasMaxLength(16).IsRequired(false);
            entity.Property(r => r.Password).HasMaxLength(128).IsRequired(false);
            entity.Property(r => r.MaxPlayers).IsRequired();
            entity.Property(r => r.Status).IsRequired();
            entity.Property(r => r.Privacy).IsRequired();

            entity.Property(r => r.Owner).IsRequired();

            // Связь: Room 1 - * Player
            entity.HasMany(r => r.Players)
                .WithOne()
                .HasForeignKey(p => p.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь: Room 1 - * Game
            entity.HasMany(r => r.Games)
                .WithOne()
                .HasForeignKey(g => g.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => r.Code).IsUnique();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("games");
            entity.HasKey(g => g.Id);

            entity.Property(g => g.RoomId).IsRequired();
            entity.Property(g => g.Mode).IsRequired();
            entity.Property(g => g.Status).IsRequired();
            entity.Property(g => g.QuestionsCount).IsRequired();
            entity.Property(g => g.QuestionDuration).IsRequired();

            entity.Property(g => g.StatisticJson)
                .IsRequired(false)
                .HasColumnType("jsonb"); // PostgreSQL

            // Связь с Room
            entity.HasOne<Room>()
                .WithMany(r => r.Games)
                .HasForeignKey(g => g.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // ← Связь Game ↔ Question через JOIN-таблицу
            entity.HasMany(g => g.Questions)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "game_questions", // Имя JOIN-таблицы
                                      // Конфигурация FK на Question
                    j => j
                        .HasOne<Question>()
                        .WithMany()
                        .HasForeignKey("question_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    // Конфигурация FK на Game
                    j => j
                        .HasOne<Game>()
                        .WithMany()
                        .HasForeignKey("game_id")
                        .OnDelete(DeleteBehavior.Cascade),
                    // Конфигурация первичного ключа JOIN-таблицы
                    j =>
                    {
                        j.HasKey("game_id", "question_id");
                        j.ToTable("game_questions");
                        j.HasIndex("game_id");
                        j.HasIndex("question_id");
                    }
                );

            entity.HasIndex(g => g.RoomId);
            entity.HasIndex(g => g.Status);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.ToTable("questions");
            entity.HasKey(q => q.Id);

            entity.Property(q => q.Formulation)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(q => q.ImageUrl)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(q => q.RightAnswer)
                .HasConversion(
                    answer => answer.Date,
                    value => new Answer(value)
                )
                .IsRequired();
        });

        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.ToTable("avatars");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Filename)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(a => a.Mimetype)
                .IsRequired()
                .HasMaxLength(50);
        });



    }
}