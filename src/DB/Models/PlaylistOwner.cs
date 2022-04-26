using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.Models;

public class PlaylistOwner
{
    //1 плейлист - создатель, но у создатель могут быть ещё плейлисты
    [Column(Order = 0), Key, ForeignKey("Playlist")]
    public int PlaylistId { get; set; }
    public Playlist Playlist { get; set; }
    
    [Column(Order = 1),  ForeignKey("UserInfo")]
    public string? UserId { get; set; } //user's id FROM User:IdentityUser
    public UserInfo UserInfo { get; set; }
}