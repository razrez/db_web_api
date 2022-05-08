using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DB.Data
{
    public class SpotifyContext : IdentityDbContext<UserInfo>
    {
        public SpotifyContext()
        {
        }

        public SpotifyContext(DbContextOptions<SpotifyContext> options)
            : base(options)
        {
        }

        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<Premium> Premia { get; set; } = null!;
        public DbSet<Profile> Profiles { get; set; } = null!;
        public DbSet<Song> Songs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("uuid-ossp");
            //Entities for testing
            var user = new UserInfo()
                    {
                        Id = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                        UserName = "user01@gmail.com",
                        NormalizedUserName = "USER01@GMAIL.COM",
                        Email = "user01@gamil.com",
                        NormalizedEmail = "USER01@GMAIL.COM",
                        ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                        PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                        SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                        EmailConfirmed = true
                    };
            modelBuilder.Entity<UserInfo>().HasData(user);
            
            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasMany(p => p.Songs)
                    .WithOne(u => u.User)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_song");

                entity.HasMany(p => p.Playlists)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "LikedPlaylist" ,
                        l => l.HasOne<Playlist>().WithMany().HasForeignKey("PlaylistId")
                            .HasConstraintName("fk_liked_playlist_playlist_id"),
                        r => r.HasOne<UserInfo>().WithMany().HasForeignKey("UserId")
                            .HasConstraintName("fk_liked_playlist_user_id"),
                        j =>
                        {
                            j.HasKey("UserId", "PlaylistId").HasName("liked_playlist_pkey");
                            j.ToTable("liked_playlist");
                            j.IndexerProperty<string>("UserId").HasColumnName("user_id");
                            j.IndexerProperty<int>("PlaylistId").HasColumnName("playlist_id");
                        });
                entity.HasData(
                    new UserInfo[]
                    {
                        new UserInfo()
                        {
                            Id = "5f34130c-2ed9-4c83-a600-e474e8f43bac",
                            UserName = "user03@gmail.com",
                            NormalizedUserName = "USER03@GMAIL.COM",
                            Email = "user03@gamil.com",
                            NormalizedEmail = "USER03@GMAIL.COM",
                            ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                            PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                            SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                            EmailConfirmed = true
                        },
                        
                        new UserInfo()
                        {
                            Id = "5f34130c-2ed9-4c83-a600-e474e8f44bac",
                            UserName = "user04@gmail.com",
                            NormalizedUserName = "USER04@GMAIL.COM",
                            Email = "user04@gamil.com",
                            NormalizedEmail = "USER04@GMAIL.COM",
                            ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                            PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                            SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                            EmailConfirmed = true
                        },
                    });
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasOne(d => d.Playlist)
                    .WithMany()
                    .HasForeignKey(d => d.PlaylistId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_genre");
            });

            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.HasKey(k => k.Id);

                entity.Property(e => e.Id).UseIdentityAlwaysColumn();

                //индекс таблица Плейлист-Песня
                entity.HasMany(d => d.Songs)
                    .WithMany(p => p.Playlists)
                    .UsingEntity<Dictionary<string, object>>(
                        "PlaylistSong",
                        l => l.HasOne<Song>().WithMany()
                            .HasForeignKey("SongId").HasConstraintName("fk_playlist_song_song_id"),
                        r => r.HasOne<Playlist>().WithMany()
                            .HasForeignKey("PlaylistId").HasConstraintName("fk_playlist_song_playlist_id"),
                        j =>
                        {
                            j.HasKey("PlaylistId", "SongId").HasName("playlist_song_pkey");

                            j.ToTable("playlist_song");

                            j.IndexerProperty<int>("PlaylistId").HasColumnName("playlist_id");

                            j.IndexerProperty<int>("SongId").HasColumnName("song_id");
                        });
                
                //при создании пользователя создается басовый плейлист LikedSongs с
                //PlaylistType = PlaylistType.LikedSongs,
                entity.HasData(
                    new Playlist[]
                    {
                        new Playlist{
                            Id = 1,
                            UserId = user.Id,
                            Title = "LikedSongs",
                            PlaylistType = PlaylistType.LikedSongs,
                            ImgSrc = "src1",
                            Verified = true
                        },
                        new Playlist{
                            Id = 2,
                            UserId = user.Id,
                            Title = "simple playlist",
                            PlaylistType = PlaylistType.User,
                            ImgSrc = "src12",
                            Verified = true
                        }
                    });
            });

            modelBuilder.Entity<Premium>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("premium_pkey");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Premium)
                    .HasForeignKey<Premium>(d => d.UserId)
                    .HasConstraintName("fk_premium");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("profile_pkey");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Profile)
                    .HasForeignKey<Profile>(d => d.UserId)
                    .HasConstraintName("fk_profile");
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.Property(e => e.Id).UseIdentityAlwaysColumn();
                entity.HasData(
                    new Song[]
                    {
                        new Song{Id = 1, UserId = user.Id, Name = "song1", Source = "src1"},
                        new Song{Id = 2, UserId = user.Id, Name = "song2", Source = "src2"},
                        new Song{Id = 3, UserId = user.Id, Name = "song3", Source = "src3"},
                        new Song{Id = 4, UserId = user.Id, Name = "song4", Source = "src4"},
                        new Song{Id = 5, UserId = user.Id, Name = "song5", Source = "src5"},
                    });
            });

        }
    }
}