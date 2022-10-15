using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntertainmentHub.Data.Migrations
{
    public partial class CommentsChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieComments",
                table: "MovieComments");

            migrationBuilder.DropIndex(
                name: "IX_MovieComments_IsDeleted",
                table: "MovieComments");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MovieComments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "MovieComments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "MovieComments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieComments",
                table: "MovieComments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MovieComments_MovieId",
                table: "MovieComments",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieComments",
                table: "MovieComments");

            migrationBuilder.DropIndex(
                name: "IX_MovieComments_MovieId",
                table: "MovieComments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MovieComments");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "MovieComments");

            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "MovieComments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieComments",
                table: "MovieComments",
                columns: new[] { "MovieId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_MovieComments_IsDeleted",
                table: "MovieComments",
                column: "IsDeleted");
        }
    }
}
