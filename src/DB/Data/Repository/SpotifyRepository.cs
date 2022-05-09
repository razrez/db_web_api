using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DB.Data.Repository;

public class SpotifyRepository : ISpotifyRepository
{
    private readonly SpotifyContext _ctx;
    private readonly UserManager<UserInfo> _userManager;

    public SpotifyRepository(SpotifyContext ctx, UserManager<UserInfo> userManager)
    {
        _ctx = ctx;
        _userManager = userManager;
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
    public async Task<bool> CreatePlaylist(Playlist newPlaylist)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(newPlaylist.UserId);
            var isContain = await _ctx.Playlists.ContainsAsync(newPlaylist);
            if (!isContain && user != null)
            {
                newPlaylist.Users.Add(user);
                await _ctx.Playlists.AddAsync(newPlaylist);
                Save();
                return true;
            }

            return false;//if already exists
        }
        catch (Exception)
        {
            return false;
        }
        
    }
    
    public async Task<bool> LikePlaylist(int playlistId, string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            var playlist = await _ctx.Playlists.FindAsync(playlistId);
            //if any is empty
            if (playlist == null || user == null) return false;
            
            playlist.Users.Add(user);
            //check if already liked
            var isContain = await _ctx.Playlists.ContainsAsync(playlist);
            if (!isContain)
            {
                _ctx.Playlists.Update(playlist);
                await _ctx.SaveChangesAsync();
            }
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
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