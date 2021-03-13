using Microsoft.EntityFrameworkCore.Migrations;

namespace Mudi_DataAccess.Migrations
{
    public partial class AddCategoryDescriptionToCategoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryDescription",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryDescription",
                table: "Category");
        }
    }
}
