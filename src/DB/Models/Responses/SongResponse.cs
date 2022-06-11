namespace DB.Models.Responses;

public class SongResponse
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }
    public int OriginPlaylistId { get; set; }
    public string OriginPlaylistTitle { get; set; }
}