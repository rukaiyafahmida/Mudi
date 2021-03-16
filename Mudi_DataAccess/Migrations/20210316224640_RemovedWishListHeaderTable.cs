using Microsoft.EntityFrameworkCore.Migrations;

namespace Mudi_DataAccess.Migrations
{
    public partial class RemovedWishListHeaderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WishListHeader");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WishListHeader",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishListHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishListHeader_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WishListHeader_ApplicationUserId",
                table: "WishListHeader",
                column: "ApplicationUserId");
        }
    }
}
