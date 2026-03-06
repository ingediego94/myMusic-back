using Microsoft.EntityFrameworkCore;
using myMusic.Domain.Entities;

namespace myMusic.Infrastructure.Data;

public class AppDbContext : DbContext
{
    // Constructor:
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }

    // Tables (DbSets):
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistSong> PlaylistSongs { get; set; }
    public DbSet<PlayHistory> PlayHistories { get; set; }

    
    // Restrictions and Relationships:
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var user = modelBuilder.Entity<User>();
        user.HasIndex(u => u.Email).IsUnique();
        
        
        
        // User:
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users"); // Nombre de la tabla en plural
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.HasIndex(u => u.UserName).IsUnique();

            entity.Property(u => u.Name).HasMaxLength(50).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();
            entity.Property(u => u.UserName).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(80).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            
            // Guarda el Enum como texto (Admin, User) en lugar de 0, 1
            // entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        });

        
        // Token:
        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("Tokens");
            entity.Property(t => t.RefreshToken).HasMaxLength(500);

            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tokens)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);    // Si se borra el usuario, se borran sus sesiones
        });

        
        // Song:
        modelBuilder.Entity<Song>(entity =>
        {
            entity.ToTable("Songs");
            entity.Property(s => s.SongName).HasMaxLength(100).IsRequired();
            entity.Property(s => s.Artist).HasMaxLength(60);
            entity.Property(s => s.AudioUrl).IsRequired();
            entity.Property(s => s.PublicIdAudio).HasMaxLength(255).IsRequired();
        });

        
        // Playlist:
        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.ToTable("Playlists");
            entity.Property(p => p.PlaylistName).HasMaxLength(100).IsRequired();

            entity.HasOne(p => p.User)
                  .WithMany(u => u.Playlists)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        
        // PlaylistSong (Tabla Intermedia):
        modelBuilder.Entity<PlaylistSong>(entity =>
        {
            entity.ToTable("PlaylistSongs");

            entity.HasOne(ps => ps.Playlist)
                  .WithMany(p => p.PlaylistSongs)
                  .HasForeignKey(ps => ps.PlaylistId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ps => ps.Song)
                  .WithMany(s => s.PlaylistSongs)
                  .HasForeignKey(ps => ps.SongId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        
        // PlayHistory:
        modelBuilder.Entity<PlayHistory>(entity =>
        {
            entity.ToTable("PlayHistory");

            entity.HasOne(ph => ph.User)
                  .WithMany(u => u.PlayHistories)
                  .HasForeignKey(ph => ph.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ph => ph.Song)
                  .WithMany(s => s.PlayHistories)
                  .HasForeignKey(ph => ph.SongId)
                  .OnDelete(DeleteBehavior.Cascade);

            // IMPORTANTE: Si se borra una Playlist, el historial se mantiene con PlaylistId = NULL
            entity.HasOne(ph => ph.Playlist)
                  .WithMany(p => p.PlayHistories)
                  .HasForeignKey(ph => ph.PlaylistId)
                  .OnDelete(DeleteBehavior.SetNull); 
        });
    }
}