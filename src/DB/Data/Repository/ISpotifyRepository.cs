using DB.Models;

namespace DB.Data.Repository;

public interface ISpotifyRepository
{
    Task<IEnumerable<Song>> GetSongs(); 
    Task<IEnumerable<Playlist>> GetAllPlaylists(); 
    Task<IEnumerable<Playlist>> GetUsersPlaylists(string userId); 
    void LikeSong(UserInfo user, Song song); //adding to LikedSongs-playlist

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
    void Save();
    
}