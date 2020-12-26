using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductData.Migrations
{
    public partial class brandAndCategoryAddedFix3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Branda",
                schema: "products",
                table: "Branda");

            migrationBuilder.RenameTable(
                name: "Branda",
                schema: "products",
                newName: "Brands",
                newSchema: "products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brands",
                schema: "products",
                table: "Brands",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Brands",
                schema: "products",
                table: "Brands");

            migrationBuilder.RenameTable(
                name: "Brands",
                schema: "products",
                newName: "Branda",
                newSchema: "products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Branda",
                schema: "products",
                table: "Branda",
                column: "BrandId");
        }
    }
}
