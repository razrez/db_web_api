using DB.Models;

namespace DB.Data.Repository;

public interface ISpotifyRepository : IDisposable
{
    //Operations with songs
    Task<IEnumerable<Song>> GetSongs(); 
    Task<bool> LikeSong(int songId, string userId);
    
    //Operations with playlists
    Task<IEnumerable<Playlist>> GetAllPlaylists();
    int GetPlaylistsCount();
    Task<IEnumerable<Playlist>> GetRandomPlaylists(int count);
    Task<IEnumerable<Playlist>> GetUsersPlaylists(string userId);
    Task<bool> CreatePlaylist(Playlist newPlaylist);
    Task<bool> LikePlaylist(int playlistId, string userId); //+
    
    /// <summary>
    ///     Асинхронно возвращает Task плейлист, со всей инфорацией о пользователях и песнях
    /// </summary>
    /// <param name="playlistId"> <see cref="string" /></param>
    /// <returns>
    ///     The task result contains a <see cref="Playlist" /> 
    /// </returns>
    Task<Playlist?> GetPlaylistInfo(int playlistId);
    Task<bool> EditPlaylist(Playlist newPlaylist);
    Task<bool> DeletePlaylist(int playlistId);
    
    //Operations with users
    Task<string> GetUserName(string userId);
    Task<UserInfo?> FindUserByIdAsync(string userId);
    Task<IEnumerable<Playlist>?> GetUserLibrary(string userId);
    
    //Operations with profiles
    Task<bool> CreateProfileAsync(Profile newProfile);

    //Other operations
    Task Save();
    Task LikeAllSongs(UserInfo user);
}