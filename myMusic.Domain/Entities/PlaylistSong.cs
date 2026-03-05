namespace myMusic.Domain.Entities;

public class PlaylistSong
{
    public int Id { get; set; }
    
    public int PlaylistId { get; set; }
    public virtual Playlist Playlist { get; set; } = null!;
    
    public int SongId { get; set; }
    public virtual Song Song { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}