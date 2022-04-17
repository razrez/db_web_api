using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DB.Models
{
    [Table("song")]
    public partial class Song
    {
        public Song()
        {
            Playlists = new HashSet<Playlist>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("name")]
        [StringLength(250)]
        public string Name { get; set; } = null!;
        [Column("source")]
        [StringLength(150)]
        public string Source { get; set; } = null!;

        [ForeignKey("UserId")]
        [InverseProperty("Songs")]
        public virtual UserInfo User { get; set; } = null!;

        [ForeignKey("SongId")]
        [InverseProperty("Songs")]
        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}
