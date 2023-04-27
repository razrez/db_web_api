using DB.Models;
using Polly;

namespace DB.Data;

public class SpotifyQuery
{
    [UseOffsetPaging]   
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Song> GetSong([Service] SpotifyContext context) => context.Songs;

    [UseProjection]
    [UseFiltering]
    public IQueryable<Playlist> GetPlaylists([Service] SpotifyContext context) => context.Playlists;
    
    [UseProjection]
    [UseFiltering]
    public IQueryable<Profile> GetUsers([Service] SpotifyContext context) => context.Profiles;
}