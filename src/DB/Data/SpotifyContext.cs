using System;
using System.Collections.Generic;
using System.IO;
using DB.Models;
using DB.Models.EnumTypes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        public DbSet<Genre> Genres { get; set; } = null!;

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
                            Id = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                            UserName = "user01@gmail.com",
                            NormalizedUserName = "USER01@GMAIL.COM",
                            Email = "user01@gmail.com",
                            NormalizedEmail = "USER01@GMAIL.COM",
                            ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                            PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                            SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                            EmailConfirmed = true
                        },
                        new UserInfo()
                        {
                            Id = "120877ed-84b9-4ed5-9b87-d78965fc4fe0",
                            UserName = "user02@gmail.com",
                            NormalizedUserName = "USER02@GMAIL.COM",
                            Email = "user02@gamil.com",
                            NormalizedEmail = "USER02@GMAIL.COM",
                            ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                            PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                            SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                            EmailConfirmed = true
                        },
                        
                        new UserInfo()
                        {
                            Id = "5f34130c-2ed9-4c83-a600-e474e8f43bac",
                            UserName = "user03@gmail.com",
                            NormalizedUserName = "USER03@GMAIL.COM",
                            Email = "user03@gmail.com",
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
                            Email = "user04@gmail.com",
                            NormalizedEmail = "USER04@GMAIL.COM",
                            ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                            PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                            SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                            EmailConfirmed = true
                        },
                        new UserInfo()
                        {
                            Id = "26ecd8e6-2f2c-4385-b855-aa0a2bd8d429",
                            UserName = "artist01@gmail.com",
                            NormalizedUserName = "ARTIST01@GMAIL.COM",
                            Email = "artist01@gmail.com",
                            NormalizedEmail = "ARTIST01@GMAIL.COM",
                            ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                            PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                            SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                            EmailConfirmed = true
                        },
                        new UserInfo()
                        {
                            Id = "ca2aa01b-a215-4611-838a-f11b9552103e",
                            UserName = "artist02@gmail.com",
                            NormalizedUserName = "ARTIST02@GMAIL.COM",
                            Email = "artist02@gamil.com",
                            NormalizedEmail = "ARTIST02@GMAIL.COM",
                            ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                            PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                            SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                            EmailConfirmed = true
                        },
                        
                        new UserInfo()
                        {
                            Id = "4b89cecf-5188-44bf-94b2-6eb0cdf8da02",
                            UserName = "artist03@gmail.com",
                            NormalizedUserName = "ARTIST03@GMAIL.COM",
                            Email = "artist03@gmail.comm",
                            NormalizedEmail = "ARTIST03@GMAIL.COM",
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
                        
                //при создании пользователя создается базовый плейлист LikedSongs с
                //PlaylistType = PlaylistType.LikedSongs,
                entity.HasData(
                    new Playlist[]
                    {
                        new Playlist{
                            Id = 1,
                            UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                            Title = "LikedSongs",
                            PlaylistType = PlaylistType.LikedSongs,
                            GenreType = GenreType.Country,
                            ImgSrc = "src1",
                            Verified = true
                        },
                        new Playlist{
                            Id = 2,
                            UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                            Title = "simple playlist",
                            PlaylistType = PlaylistType.User,
                            GenreType = GenreType.Electro,
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
                entity.HasData(
                    new Profile[]
                    {
                        new Profile()
                        {
                            UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                            Username = "user01",
                            Birthday = new DateOnly(2000, 8, 3),
                            Country = Country.Russia,
                            ProfileImg = "src1",
                            UserType = UserType.User
                        },
                        new Profile()
                        {
                            UserId = "120877ed-84b9-4ed5-9b87-d78965fc4fe0",
                            Username = "user02",
                            Birthday = new DateOnly(1996, 2, 23),
                            Country = Country.Greece,
                            ProfileImg = "src2",
                            UserType = UserType.User
                        },
                        new Profile()
                        {
                            UserId = "5f34130c-2ed9-4c83-a600-e474e8f43bac",
                            Username = "user03",
                            Birthday = new DateOnly(2008, 3, 16),
                            Country = Country.Russia,
                            ProfileImg = "src3",
                            UserType = UserType.User
                        },
                        new Profile()
                        {
                            UserId = "5f34130c-2ed9-4c83-a600-e474e8f44bac",
                            Username = "user04",
                            Birthday = new DateOnly(1987, 2, 21),
                            Country = Country.Usa,
                            ProfileImg = "src4",
                            UserType = UserType.User
                        },
                        new Profile()
                        {
                            UserId = "26ecd8e6-2f2c-4385-b855-aa0a2bd8d429",
                            Username = "artist10",
                            Birthday = new DateOnly(1985, 2, 21),
                            Country = Country.Russia,
                            ProfileImg = "src5",
                            UserType = UserType.Artist
                        },
                        new Profile()
                        {
                            UserId = "ca2aa01b-a215-4611-838a-f11b9552103e",
                            Username = "artist02",
                            Birthday = new DateOnly(1998, 2, 21),
                            Country = Country.Usa,
                            ProfileImg = "src6",
                            UserType = UserType.Artist
                        },
                        new Profile()
                        {
                            UserId = "4b89cecf-5188-44bf-94b2-6eb0cdf8da02",
                            Username = "artist03",
                            Birthday = new DateOnly(1995, 2, 21),
                            Country = Country.Usa,
                            ProfileImg = "src7",
                            UserType = UserType.Artist
                        },
                    }
                    );
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.Property(e => e.Id).UseIdentityAlwaysColumn();
                entity.HasData(
                    new Song[]
                    {
                        new Song{Id = 1, UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac", Name = "song1", Source = "src1"},
                        new Song{Id = 2, UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac", Name = "song2", Source = "src2"},
                        new Song{Id = 3, UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac", Name = "song3", Source = "src3"},
                        new Song{Id = 4, UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac", Name = "song4", Source = "src4"},
                        new Song{Id = 5, UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac", Name = "song5", Source = "src5"},
                    });
            });
            var profile = new Profile()
            {
                UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                Username = "user01@gmail.com",
                Birthday = new DateOnly(2020, 01, 01),
                Country = Country.Ukraine,
                UserType = UserType.User,
            };

            var premium = new Premium()
            {
                UserId = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                PremiumType = PremiumType.Student,
                StartAt = new DateTime(2020, 1, 1),
                EndAt = new DateTime(2020, 6, 6)
            };


            //modelBuilder.Entity<UserInfo>().HasData(user);
            //modelBuilder.Entity<Profile>().HasData(profile);
            //modelBuilder.Entity<Premium>().HasData(premium);
        }
    }
}