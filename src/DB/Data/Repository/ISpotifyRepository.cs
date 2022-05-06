using DB.Models;
using DB.Models.EnumTypes;

namespace DB.Data.Repository;

public interface ISpotifyRepository
{
    Task<IEnumerable<Song>> GetSongs(); //+
    Task<IEnumerable<Playlist>> GetAllPlaylists(); //+
    Task<IEnumerable<Playlist>> GetUsersPlaylists(string userId); //+
    void LikePlaylist(UserInfo user, Playlist playlist); //+
    void LikeSong(UserInfo user, Song song); //adding to LikedSongs-playlist

    //операции с плейлистами
    void CreatePlaylist(UserInfo user, string title, PlaylistType playlistType, string? imgSrc);
    
    /// <summary>
    ///     Асинхронно возвращает Task плейлист, со всей инфорацией о пользователях и песнях
    /// </summary>
    /// <param name="playlistId"> <see cref="string" /></param>
    /// <returns>
    ///     The task result contains a <see cref="Playlist" /> 
    /// </returns>
    Task<Playlist?> GetPlaylistInfo(int playlistId);
    
    void Save();//+ 
    
}