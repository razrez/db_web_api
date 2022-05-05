using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.EntityFrameworkCore;

namespace DB.Data.Repository;

public class SpotifyRepository : ISpotifyRepository
{
    private readonly SpotifyContext _ctx;

    public SpotifyRepository(SpotifyContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IEnumerable<Song>> GetSongs()
    {
        var songs = await _ctx.Songs.ToListAsync();
        return songs;
    }
    
    public async Task<IEnumerable<Playlist>> GetAllPlaylists()
    {
        var playlists = await _ctx.Playlists.ToListAsync();
        return playlists;
    }

    public async Task<IEnumerable<Playlist>> GetUsersPlaylists(string userId)
    {
        var usersPlaylists = await _ctx.Playlists
            .Include(x => x.Songs)
            .Include(x => x.Users)
            .AsSplitQuery()
            .Where(k => k.UserId == userId)
            .ToListAsync();
        return usersPlaylists;
    }
    
    public void LikePlaylist(UserInfo user, Playlist playlist)
    {
        playlist.Users.Add(user);
        _ctx.Playlists.Update(playlist);
        Save();
    }

    public async void LikeSong(UserInfo user, Song song)
    {
        var likedSongsPlaylist = await _ctx.Playlists
            .Include(x => x.Songs)
            .Include(x => x.Users)
            .AsSplitQuery()
            .Where(k => k.UserId == user.Id && k.PlaylistType == PlaylistType.LikedSongs)
            .FirstAsync();
        likedSongsPlaylist.Songs.Add(song);
        _ctx.Playlists.Update(likedSongsPlaylist);
        Save();
    }

    public void AddSongToPlaylist(UserInfo user, Playlist playlist)
    {
        
        throw new NotImplementedException();
    }

    public async void Save()
    {
        await _ctx.SaveChangesAsync();
    }
}