namespace myMusic.Domain.Entities;

public class PlayHistory
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public int? PlaylistId { get; set; }
    public virtual Playlist? Playlist { get; set; }
    
    public int SongId { get; set; }
    public virtual Song Song { get; set; } = null!;
    
    public DateTime PlayedAt { get; set; }
}