using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace formigrations.Migrations
{
    /// <inheritdoc />
    public partial class addfollowersCounttoblog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FollowersCount",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FollowersCount",
                table: "Blogs");
        }
    }
}
