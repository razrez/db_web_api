using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DB.Models;

namespace DB.Models;

public class LikedPlaylist
{
    [Column(Order = 0), Key, ForeignKey("Room")]
    public int PlaylistId { get; set; } 
    public Playlist? Playlist { get; set; } // navigation property
    
    [Column(Order = 1),Key]
    public string? UserId { get; set; } 
    public UserInfo? User { get; set; }
}