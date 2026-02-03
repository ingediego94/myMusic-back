namespace myMusic.Domain.Entities;

public class Song
{
    public int Id { get; set; }
    public string SongName { get; set; }
    public string? Artist { get; set; }
    public int Duration { get; set; }
    public string AudioUrl { get; set; }
    public string PublicIdAudio { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Relations:
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}