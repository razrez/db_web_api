using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DB.Models
{
    [Table("user_info")]
    public partial class UserInfo
    {
        public UserInfo()
        {
            PlaylistsNavigation = new HashSet<Playlist>();
            Songs = new HashSet<Song>();
            Playlists = new HashSet<Playlist>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; } = null!;
        
        [Column("password")]
        [StringLength(255)]
        public string Password { get; set; } = null!;
        

        [InverseProperty("User")]
        public virtual Premium Premium { get; set; } = null!;
        
        [InverseProperty("User")]
        public virtual Profile Profile { get; set; } = null!;
        
        [InverseProperty("User")]
        public virtual ICollection<Playlist> PlaylistsNavigation { get; set; }
        
        [InverseProperty("User")]
        public virtual ICollection<Song> Songs { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Users")]
        public virtual ICollection<Playlist> Playlists { get; set; }
    }
}
