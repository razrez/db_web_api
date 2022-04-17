using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DB.Migrations
{
    public partial class Configuring2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:country", "russia,ukraine,usa,greece")
                .Annotation("Npgsql:Enum:genre_type", "rock,jazz,techno,electro,country,pop")
                .Annotation("Npgsql:Enum:playlist_type", "album,single,ep,user,liked_songs")
                .Annotation("Npgsql:Enum:premium_type", "individual,student,duo,family,basic")
                .Annotation("Npgsql:Enum:user_type", "user,artist,admin");

            migrationBuilder.CreateTable(
                name: "PlaylistUserInfo",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistUserInfo", x => new { x.PlaylistId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "user_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_info", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "playlist",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    playlist_type = table.Column<int>(type: "integer", nullable: false),
                    img_src = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    verified = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playlist", x => x.id);
                    table.ForeignKey(
                        name: "fk_playlist",
                        column: x => x.user_id,
                        principalTable: "user_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "premium",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    premium_type = table.Column<int>(type: "integer", nullable: false),
                    start_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("premium_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_premium",
                        column: x => x.user_id,
                        principalTable: "user_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    birthday = table.Column<DateOnly>(type: "date", nullable: true),
                    country = table.Column<int>(type: "integer", nullable: true),
                    profile_img = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    user_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("profile_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_profile",
                        column: x => x.user_id,
                        principalTable: "user_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "song",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    source = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song", x => x.id);
                    table.ForeignKey(
                        name: "fk_song",
                        column: x => x.user_id,
                        principalTable: "user_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "genre",
                columns: table => new
                {
                    playlist_id = table.Column<int>(type: "integer", nullable: true),
                    genre_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "fk_genre",
                        column: x => x.playlist_id,
                        principalTable: "playlist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "liked_playlist",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    playlist_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("liked_playlist_pkey", x => new { x.user_id, x.playlist_id });
                    table.ForeignKey(
                        name: "fk_liked_playlist_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_liked_playlist_user_id",
                        column: x => x.user_id,
                        principalTable: "user_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "playlist_song",
                columns: table => new
                {
                    playlist_id = table.Column<int>(type: "integer", nullable: false),
                    song_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("playlist_song_pkey", x => new { x.playlist_id, x.song_id });
                    table.ForeignKey(
                        name: "fk_playlist_song_playlist_id",
                        column: x => x.playlist_id,
                        principalTable: "playlist",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_playlist_song_song_id",
                        column: x => x.song_id,
                        principalTable: "song",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_genre_playlist_id",
                table: "genre",
                column: "playlist_id");

            migrationBuilder.CreateIndex(
                name: "IX_liked_playlist_playlist_id",
                table: "liked_playlist",
                column: "playlist_id");

            migrationBuilder.CreateIndex(
                name: "IX_playlist_user_id",
                table: "playlist",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_playlist_song_song_id",
                table: "playlist_song",
                column: "song_id");

            migrationBuilder.CreateIndex(
                name: "IX_song_user_id",
                table: "song",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "genre");

            migrationBuilder.DropTable(
                name: "liked_playlist");

            migrationBuilder.DropTable(
                name: "playlist_song");

            migrationBuilder.DropTable(
                name: "PlaylistUserInfo");

            migrationBuilder.DropTable(
                name: "premium");

            migrationBuilder.DropTable(
                name: "profile");

            migrationBuilder.DropTable(
                name: "playlist");

            migrationBuilder.DropTable(
                name: "song");

            migrationBuilder.DropTable(
                name: "user_info");
        }
    }
}
