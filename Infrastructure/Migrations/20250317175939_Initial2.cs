using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserRole_UserRoleID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserRoleID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserRoleID",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserRoleID",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserRoleID",
                table: "AspNetUsers",
                column: "UserRoleID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserRole_UserRoleID",
                table: "AspNetUsers",
                column: "UserRoleID",
                principalTable: "UserRole",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
