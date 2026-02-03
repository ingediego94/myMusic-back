namespace myMusic.Domain.Entities;

public class PlayHistory
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }

    public int PlaylistId { get; set; }
    public Playlist Playlist { get; set; }
    
    public int SongId { get; set; }
    public Song Song { get; set; }
    
    public DateTime PlayedAt { get; set; }
}