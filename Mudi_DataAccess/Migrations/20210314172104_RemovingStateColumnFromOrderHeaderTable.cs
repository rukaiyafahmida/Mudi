using Microsoft.EntityFrameworkCore.Migrations;

namespace Mudi_DataAccess.Migrations
{
    public partial class RemovingStateColumnFromOrderHeaderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "OrderHeader");

            migrationBuilder.RenameColumn(
                name: "Oty",
                table: "OrderDetail",
                newName: "Qty");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "OrderDetail",
                newName: "Oty");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
