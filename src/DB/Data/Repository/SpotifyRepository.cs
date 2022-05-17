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
    
    //Operations with songs
    public async Task<IEnumerable<Song>> GetSongs()
    {
        return await _ctx.Songs.ToListAsync();
    }
    public async Task<bool> LikeSong(int songId, string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            var song = await _ctx.Songs.FindAsync(songId);
            var likedSongsPlaylist = await _ctx.Playlists
                .Where(k => k.UserId == userId && k.PlaylistType == PlaylistType.LikedSongs)
                .FirstOrDefaultAsync();
            
            //if any is empty
            if (likedSongsPlaylist == null || song == null || user == null) return false;

            likedSongsPlaylist.Songs.Add(song);
            //likedSongsPlaylist.Users.Add(user);
            _ctx.Playlists.Update(likedSongsPlaylist);
            await _ctx.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public async Task<List<Song>> SearchSongs(string input)
    {
        var result = await _ctx.Songs
            .Where(p => p.Name.ToUpper().Contains(input.ToUpper()))
            .ToListAsync();
        return result;
    }
    
    //Operations with playlists
    public async Task<IEnumerable<Playlist>> GetAllPlaylists()
    { 
        return await _ctx.Playlists.ToListAsync();
    }

    public int GetPlaylistsCount()
    {
        return _ctx.Playlists.Count();
    }

    public async Task<IEnumerable<Playlist>> GetRandomPlaylists(int count)
    {
        return await _ctx.Playlists.OrderBy(r => Guid.NewGuid()).Take(count).ToListAsync();
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
    
    public async Task<bool> CreatePlaylist(Playlist newPlaylist)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(newPlaylist.UserId);
            
            var test = new Playlist
            {
                UserId = user.Id,
                Title = newPlaylist.Title,
                PlaylistType = newPlaylist.PlaylistType,
                ImgSrc = "src3",
                Verified = true
            };
            
            test.Users.Add(user);
            await _ctx.AddAsync(test);
            await _ctx.SaveChangesAsync();
            return true;
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
            _ctx.Playlists.Update(playlist);
            await _ctx.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<Playlist>> SearchPlaylists(string input)
    {
        
        var result = await _ctx.Playlists
            .Where(p => p.Title.ToUpper().Contains(input.ToUpper()))
            .ToListAsync();
        return result;
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
                await _ctx.SaveChangesAsync();
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
            if (currentPlaylist != null)
            {
                _ctx.Remove(currentPlaylist);
                
                await _ctx.SaveChangesAsync();
                return true;
            }
            return false;
            
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    //Operations with users
    public async Task<string> GetUserName(string userId)
    {
        var name = await _userManager.FindByIdAsync(userId);
        return name.UserName;
    }

    public async Task<UserInfo?> FindUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);  
    } 
    
    public async Task<IEnumerable<Playlist>?> GetUserLibrary(string userId)
    {
        var userLibrary = await _ctx.Users
            .Include(p => p.Playlists)
            .ThenInclude(s => s.Songs)
            .AsSplitQuery()
            .Where(u => u.Id == userId)
            .ToListAsync();
        return userLibrary.SelectMany(s => s.Playlists);
    }
    
    //Operations with profiles
    public async Task<bool> CreateProfileAsync(Profile newProfile)
    {
        try
        {
            await _ctx.Profiles.AddAsync(newProfile);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<Profile>> SearchProfile(string input, bool isArtist)
    {
        var userType = UserType.User;
        if (isArtist)
            userType = UserType.Artist;
        var result = await _ctx.Profiles
            .Where(p => p.UserType == userType)
            .Where(p => p.Username.ToUpper().Contains(input.ToUpper()))
            .ToListAsync();
        return result;
    }

    //Other operations
    public async Task Save()
    {
        await _ctx.SaveChangesAsync();
    }
    public async Task LikeAllSongs(UserInfo user)
    {
        var songs = await _ctx.Songs.ToListAsync();
        var playlist = await _ctx.Playlists.FirstAsync();
        //like song
        foreach (var song in songs) playlist.Songs.Add(song);
        playlist.Users.Add(user); //нужно чтобы по дефолту при
        //создании пользователя у него был плейлист LikedSongs
        //а при создании плейлиста пользователем надо их связать через индекс LikedPlaylists
        _ctx.Playlists.Update(playlist);
        await Save();
        /*var isContain = await _ctx.Playlists.ContainsAsync(playlist);
        if (!isContain)
        {
            _ctx.Playlists.Update(playlist);
            await _ctx.SaveChangesAsync();
        }*/
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _userManager.Dispose();
    }
}