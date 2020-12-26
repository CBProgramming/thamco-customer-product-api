using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductData.Migrations
{
    public partial class schemaAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Products");

            migrationBuilder.EnsureSchema(
                name: "products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "products");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "products",
                table: "Products",
                newName: "Quantity");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                schema: "products",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                schema: "products",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                schema: "products",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                schema: "products",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "products",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                schema: "products",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                schema: "products",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                schema: "products",
                table: "Products",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                schema: "products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Brand",
                schema: "products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BrandId",
                schema: "products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "products",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "products",
                newName: "Products");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Products",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<float>(
                name: "Value",
                table: "Products",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");
        }
    }
}
