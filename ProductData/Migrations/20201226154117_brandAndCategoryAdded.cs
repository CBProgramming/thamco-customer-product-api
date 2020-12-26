using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductData.Migrations
{
    public partial class brandAndCategoryAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                schema: "products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "products",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "Brand",
                schema: "products",
                columns: table => new
                {
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.BrandId);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "products",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Brand",
                schema: "products");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "products");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                schema: "products",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "products",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
