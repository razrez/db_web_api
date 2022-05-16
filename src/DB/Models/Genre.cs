using System.ComponentModel.DataAnnotations.Schema;
using DB.Models.EnumTypes;
using Microsoft.EntityFrameworkCore;

namespace DB.Models
{
    [Keyless]
    [Table("genre")]
    public class Genre
    {
        [Column("playlist_id")]
        public int? PlaylistId { get; set; }

        [Column("genre_type")] 
        public GenreType GenreType { get; set; }

        [ForeignKey("PlaylistId")]
        public virtual Playlist? Playlist { get; set; }
    }
}
