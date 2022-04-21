using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DB.Models
{
    [Table("user_info")]
    public sealed partial class UserInfo:IdentityUser
    {
        public UserInfo()
        {
            PlaylistsNavigation = new HashSet<Playlist>();
            Songs = new HashSet<Song>();
            Playlists = new HashSet<Playlist>();
        }

        [Key]
        [Column("id")]
        public override string Id { get; set; } = null!;

        [Column("email")]
        [StringLength(255)]
        public override string Email { get; set; } = null!;

        [InverseProperty("User")]
        public Premium Premium { get; set; } = null!;

        [InverseProperty("User")]
        public Profile Profile { get; set; } = null!;

        [InverseProperty("User")]
        public ICollection<Playlist> PlaylistsNavigation { get; set; }
        
        [InverseProperty("User")]
        public ICollection<Song> Songs { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Users")]
        public ICollection<Playlist> Playlists { get; set; }
    }
}
