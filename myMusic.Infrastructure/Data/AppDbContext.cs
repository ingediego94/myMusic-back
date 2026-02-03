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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();
        user.HasIndex(u => u.Email).IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }
    
    
    // Tables:
    public DbSet<PlayHistory> PlayHistories { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistSong> PlaylistSongs { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<User> Users { get; set; }
}