namespace myMusic.Domain.Entities;

public class Song
{
    public int Id { get; set; }
    public string SongName { get; set; } = string.Empty;
    public string? Artist { get; set; }
    public int Duration { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public string PublicIdAudio { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Relations:
    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
    public virtual ICollection<PlayHistory> PlayHistories { get; set; } = new List<PlayHistory>();
}