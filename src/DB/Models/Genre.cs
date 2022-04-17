using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DB.Models
{
    [Keyless]
    [Table("genre")]
    public partial class Genre
    {
        [Column("playlist_id")]
        public int? PlaylistId { get; set; }

        [ForeignKey("PlaylistId")]
        public virtual Playlist? Playlist { get; set; }
    }
}
