using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xenon.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    JoinedChannelID = table.Column<ulong>(type: "INTEGER", nullable: true),
                    LeftChannelID = table.Column<ulong>(type: "INTEGER", nullable: true),
                    JoinOrLeaveDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelChanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Moderator = table.Column<ulong>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Expiry = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    AppliedRoleID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mutes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelChanges");

            migrationBuilder.DropTable(
                name: "Mutes");
        }
    }
}
