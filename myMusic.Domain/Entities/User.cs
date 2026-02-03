using myMusic.Domain.Enums;

namespace myMusic.Domain.Entities;

public class User
{
    public  int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsActive { get; set; }
    public Role Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Inverse relations:
    public ICollection<Token> Tokens { get; set; } = new List<Token>();
    public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    public ICollection<PlayHistory> PlayHistories { get; set; } = new List<PlayHistory>();
}