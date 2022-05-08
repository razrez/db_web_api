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

    public async Task<IEnumerable<Song>> GetSongs() => await _ctx.Songs.ToListAsync();
    
    public async Task<IEnumerable<Playlist>> GetAllPlaylists() => await _ctx.Playlists.ToListAsync();
    
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

    //операции с плейлистами
    public async void CreatePlaylist(UserInfo user, string title, PlaylistType playlistType, string? imgSrc)
    {
        var newPlaylist = new Playlist
        {
            UserId = user.Id,
            Title = title,
            PlaylistType = playlistType,
            ImgSrc = imgSrc,
            Verified = playlistType is PlaylistType.User or PlaylistType.LikedSongs
        };
        newPlaylist.Users.Add(user);
        await _ctx.Playlists.AddAsync(newPlaylist);
        Save();
    }
    
    public void LikePlaylist(UserInfo user, Playlist playlist)
        {
            playlist.Users.Add(user);
            _ctx.Playlists.Update(playlist);
            Save();
        }
    
    public async Task<Playlist?> GetPlaylistInfo(int playlistId)
    {
        var playlist = await _ctx.Playlists
            .Include(x => x.Songs)
            .Include(x => x.Users)
            .AsSplitQuery()
            .Where(k => k.Id == playlistId)
            .FirstOrDefaultAsync();
        return playlist;
    }


    public async Task<bool> EditPlaylist(Playlist newPlaylist)
    {
        try
        {
            var ctxPlaylist = await _ctx.Playlists.FindAsync(newPlaylist.Id);
            if (ctxPlaylist != null)
            {
                ctxPlaylist.Title = newPlaylist.Title;
                if (newPlaylist.ImgSrc != null && newPlaylist.ImgSrc != ctxPlaylist.ImgSrc)
                {
                    ctxPlaylist.ImgSrc = newPlaylist.ImgSrc;
                }
                _ctx.Playlists.Update(ctxPlaylist);
                Save();
                return true;
            }
            
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeletePlaylist(int playlistId)
    {
        try
        {
            var currentPlaylist = await _ctx.Playlists.FindAsync(playlistId);
            if (currentPlaylist != null) _ctx.Playlists.RemoveRange(currentPlaylist);
            Save();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async void Save()
    {
        await _ctx.SaveChangesAsync();
    }
}