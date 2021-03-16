using Microsoft.EntityFrameworkCore.Migrations;

namespace Mudi_DataAccess.Migrations
{
    public partial class AddingApplicationUserIdtoWishList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishListDetail_WishListHeader_WishListHeaderId",
                table: "WishListDetail");

            migrationBuilder.DropIndex(
                name: "IX_WishListDetail_WishListHeaderId",
                table: "WishListDetail");

            migrationBuilder.DropColumn(
                name: "WishListHeaderId",
                table: "WishListDetail");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "WishListDetail",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WishListDetail_ApplicationUserId",
                table: "WishListDetail",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishListDetail_AspNetUsers_ApplicationUserId",
                table: "WishListDetail",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishListDetail_AspNetUsers_ApplicationUserId",
                table: "WishListDetail");

            migrationBuilder.DropIndex(
                name: "IX_WishListDetail_ApplicationUserId",
                table: "WishListDetail");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "WishListDetail");

            migrationBuilder.AddColumn<int>(
                name: "WishListHeaderId",
                table: "WishListDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WishListDetail_WishListHeaderId",
                table: "WishListDetail",
                column: "WishListHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_WishListDetail_WishListHeader_WishListHeaderId",
                table: "WishListDetail",
                column: "WishListHeaderId",
                principalTable: "WishListHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
