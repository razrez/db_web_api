using DB.Models;

namespace DB.Data.Repository;

public interface ISpotifyRepository
{
    //Task<IEnumerable<SpotifyContext>> GetAllContext();
    Task<IEnumerable<Song>> GetSongs(); //+
    Task<IEnumerable<Playlist>> GetAllPlaylists(); //+
    Task<IEnumerable<Playlist>> GetUsersPlaylists(string userId); //+
    void LikePlaylist(UserInfo user, Playlist playlist);
    void LikeSong(UserInfo user, Song song); //adding to LikedSongs-playlist
    void AddSongToPlaylist(UserInfo user, Playlist playlist);
    void Save();//+
    
}