using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductData.Migrations
{
    public partial class brandAndCategoryAddedFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                schema: "products",
                table: "Category");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Brand",
                schema: "products",
                table: "Brand");

            migrationBuilder.RenameTable(
                name: "Category",
                schema: "products",
                newName: "Categories",
                newSchema: "products");

            migrationBuilder.RenameTable(
                name: "Brand",
                schema: "products",
                newName: "Branda",
                newSchema: "products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                schema: "products",
                table: "Categories",
                column: "CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Branda",
                schema: "products",
                table: "Branda",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                schema: "products",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Branda",
                schema: "products",
                table: "Branda");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "products",
                newName: "Category",
                newSchema: "products");

            migrationBuilder.RenameTable(
                name: "Branda",
                schema: "products",
                newName: "Brand",
                newSchema: "products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                schema: "products",
                table: "Category",
                column: "CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brand",
                schema: "products",
                table: "Brand",
                column: "BrandId");
        }
    }
}
