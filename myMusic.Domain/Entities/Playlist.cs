namespace myMusic.Domain.Entities;

public class Playlist
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public string PlaylistName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Inverse relations:
    public ICollection<PlayHistory> PlayHistories { get; set; } = new List<PlayHistory>();
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}