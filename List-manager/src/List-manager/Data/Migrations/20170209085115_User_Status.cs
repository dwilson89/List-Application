using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace List_manager.Data.Migrations
{
    public partial class User_Status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "UserAnime",
                columns: table => new
                {
                    UserAnimeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnimeID = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    User_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnime", x => x.UserAnimeID);
                    table.ForeignKey(
                        name: "FK_UserAnime_Anime_AnimeID",
                        column: x => x.AnimeID,
                        principalTable: "Anime",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAnime_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

          

            migrationBuilder.CreateIndex(
                name: "IX_UserAnime_AnimeID",
                table: "UserAnime",
                column: "AnimeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnime_ApplicationUserId",
                table: "UserAnime",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MALID",
                table: "Anime");

            migrationBuilder.DropTable(
                name: "UserAnime");

            migrationBuilder.AddColumn<int>(
                name: "DBID",
                table: "Anime",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "User_Status",
                table: "Anime",
                nullable: true);
        }
    }
}
