﻿// <auto-generated />
using System;
using DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DB.Migrations
{
    [DbContext(typeof(SpotifyContext))]
    partial class SpotifyContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "country", new[] { "russia", "ukraine", "usa", "greece" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "genre_type", new[] { "rock", "jazz", "techno", "electro", "country", "pop" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "playlist_type", new[] { "album", "single", "ep", "user", "liked_songs" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "premium_type", new[] { "individual", "student", "duo", "family", "basic" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "user_type", new[] { "user", "artist", "admin" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DB.Models.Genre", b =>
                {
                    b.Property<int>("GenreType")
                        .HasColumnType("integer")
                        .HasColumnName("genre_type");

                    b.Property<int?>("PlaylistId")
                        .HasColumnType("integer")
                        .HasColumnName("playlist_id");

                    b.HasIndex("PlaylistId");

                    b.ToTable("genre");
                });

            modelBuilder.Entity("DB.Models.Playlist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<string>("ImgSrc")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("img_src");

                    b.Property<int>("PlaylistType")
                        .HasColumnType("integer")
                        .HasColumnName("playlist_type");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("title");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<bool?>("Verified")
                        .HasColumnType("boolean")
                        .HasColumnName("verified");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("playlist");
                });

            modelBuilder.Entity("DB.Models.Premium", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("end_at");

                    b.Property<int>("PremiumType")
                        .HasColumnType("integer")
                        .HasColumnName("premium_type");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("start_at");

                    b.HasKey("UserId")
                        .HasName("premium_pkey");

                    b.ToTable("premium");
                });

            modelBuilder.Entity("DB.Models.Profile", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<DateOnly?>("Birthday")
                        .HasColumnType("date")
                        .HasColumnName("birthday");

                    b.Property<int?>("Country")
                        .HasColumnType("integer")
                        .HasColumnName("country");

                    b.Property<string>("ProfileImg")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("profile_img");

                    b.Property<int>("UserType")
                        .HasColumnType("integer")
                        .HasColumnName("user_type");

                    b.Property<string>("Username")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("username");

                    b.HasKey("UserId")
                        .HasName("profile_pkey");

                    b.ToTable("profile");
                });

            modelBuilder.Entity("DB.Models.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("name");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("source");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("song");
                });

            modelBuilder.Entity("DB.Models.UserInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("password");

                    b.HasKey("Id");

                    b.ToTable("user_info");
                });

            modelBuilder.Entity("LikedPlaylist", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("PlaylistId")
                        .HasColumnType("integer")
                        .HasColumnName("playlist_id");

                    b.HasKey("UserId", "PlaylistId")
                        .HasName("liked_playlist_pkey");

                    b.HasIndex("PlaylistId");

                    b.ToTable("liked_playlist", (string)null);
                });

            modelBuilder.Entity("PlaylistSong", b =>
                {
                    b.Property<int>("PlaylistId")
                        .HasColumnType("integer")
                        .HasColumnName("playlist_id");

                    b.Property<int>("SongId")
                        .HasColumnType("integer")
                        .HasColumnName("song_id");

                    b.HasKey("PlaylistId", "SongId")
                        .HasName("playlist_song_pkey");

                    b.HasIndex("SongId");

                    b.ToTable("playlist_song", (string)null);
                });

            modelBuilder.Entity("PlaylistUserInfo", b =>
                {
                    b.Property<int>("PlaylistId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("PlaylistId", "UserId");

                    b.ToTable("PlaylistUserInfo");
                });

            modelBuilder.Entity("DB.Models.Genre", b =>
                {
                    b.HasOne("DB.Models.Playlist", "Playlist")
                        .WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .HasConstraintName("fk_genre");

                    b.Navigation("Playlist");
                });

            modelBuilder.Entity("DB.Models.Playlist", b =>
                {
                    b.HasOne("DB.Models.UserInfo", "User")
                        .WithMany("PlaylistsNavigation")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_playlist");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DB.Models.Premium", b =>
                {
                    b.HasOne("DB.Models.UserInfo", "User")
                        .WithOne("Premium")
                        .HasForeignKey("DB.Models.Premium", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_premium");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DB.Models.Profile", b =>
                {
                    b.HasOne("DB.Models.UserInfo", "User")
                        .WithOne("Profile")
                        .HasForeignKey("DB.Models.Profile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_profile");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DB.Models.Song", b =>
                {
                    b.HasOne("DB.Models.UserInfo", "User")
                        .WithMany("Songs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_song");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LikedPlaylist", b =>
                {
                    b.HasOne("DB.Models.Playlist", null)
                        .WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_liked_playlist_playlist_id");

                    b.HasOne("DB.Models.UserInfo", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_liked_playlist_user_id");
                });

            modelBuilder.Entity("PlaylistSong", b =>
                {
                    b.HasOne("DB.Models.Playlist", null)
                        .WithMany()
                        .HasForeignKey("PlaylistId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_playlist_song_playlist_id");

                    b.HasOne("DB.Models.Song", null)
                        .WithMany()
                        .HasForeignKey("SongId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_playlist_song_song_id");
                });

            modelBuilder.Entity("DB.Models.UserInfo", b =>
                {
                    b.Navigation("PlaylistsNavigation");

                    b.Navigation("Premium")
                        .IsRequired();

                    b.Navigation("Profile")
                        .IsRequired();

                    b.Navigation("Songs");
                });
#pragma warning restore 612, 618
        }
    }
}
