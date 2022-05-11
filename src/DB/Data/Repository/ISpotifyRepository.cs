using DB.Models;

namespace DB.Data.Repository;

public interface ISpotifyRepository : IDisposable
{
    Task<IEnumerable<Song>> GetSongs(); 
    Task<IEnumerable<Playlist>> GetAllPlaylists();
    int GetPlaylistsCount();
    Task<IEnumerable<Playlist>> GetRandomPlaylists(int count);
    Task<IEnumerable<Playlist>> GetUsersPlaylists(string userId); 
    Task<bool> LikeSong(int songId, string userId); //adding to LikedSongs-playlist
    Task<string> GetUserName(string userId);
    Task<UserInfo?> FindUserByIdAsync(string userId);

    //операции с плейлистами
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
    Task<IEnumerable<Playlist>?> GetUserLibrary(string userId);
    Task Save();
    Task LikeAllSongs(UserInfo user);

    //операции с профилем
    Task<bool> CreateProfileAsync(Profile newProfile);
}