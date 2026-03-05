namespace myMusic.Domain.Entities;

public class Playlist
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public string PlaylistName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Inverse relations:
    public virtual ICollection<PlayHistory> PlayHistories { get; set; } = new List<PlayHistory>();
    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}