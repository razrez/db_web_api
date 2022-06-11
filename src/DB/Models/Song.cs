using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DB.Models
{
    [Table("song")]
    public sealed class Song
    {
        public Song()
        {
            Playlists = new HashSet<Playlist>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("user_id")]
        public string UserId { get; set; }
        
        // в бд по дефолту значение = 0
        [Column("origin_playlist_id")]
        public int OriginPlaylistId { get; set; }
        
        [Column("name")]
        [StringLength(250)]
        public string Name { get; set; } = null!;
        
        [Column("source")]
        [StringLength(150)]
        public string Source { get; set; } = null!;
        
        [JsonIgnore]
        [ForeignKey("UserId")]
        [InverseProperty("Songs")]
        public UserInfo User { get; set; } = null!;

        [JsonIgnore]
        [ForeignKey("SongId")]
        [InverseProperty("Songs")]
        public ICollection<Playlist> Playlists { get; set; }
    }
}
