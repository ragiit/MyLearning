using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Apple.Services.ProductAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryName", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Wearable", "Smartwatch dari Apple", "https://placehold.co/603x403", "Apple Watch", 299.0 },
                    { 2, "Smartphone", "Smartphone terbaru dari Apple", "https://placehold.co/603x403", "iPhone 15", 999.0 },
                    { 3, "Laptop", "Laptop performa tinggi", "https://placehold.co/603x403", "MacBook Pro", 1999.0 },
                    { 4, "Tablet", "Tablet ringan dan cepat", "https://placehold.co/603x403", "iPad Air", 599.0 },
                    { 5, "Aksesoris", "Earbuds nirkabel dengan noise cancelling", "https://placehold.co/603x403", "AirPods Pro", 249.0 },
                    { 6, "Desktop", "All-in-one desktop komputer", "https://placehold.co/603x403", "iMac 24\"", 1299.0 },
                    { 7, "Aksesoris", "Stylus untuk iPad", "https://placehold.co/603x403", "Apple Pencil", 129.0 },
                    { 8, "Aksesoris", "Mouse wireless dari Apple", "https://placehold.co/603x403", "Magic Mouse", 99.0 },
                    { 9, "Smart Home", "Speaker pintar dari Apple", "https://placehold.co/603x403", "HomePod Mini", 99.0 },
                    { 10, "Entertainment", "Streaming device 4K", "https://placehold.co/603x403", "Apple TV 4K", 179.0 },
                    { 11, "Desktop", "Komputer kecil namun bertenaga", "https://placehold.co/603x403", "Mac Mini", 699.0 },
                    { 12, "Aksesoris", "Pelacak barang pintar", "https://placehold.co/603x403", "AirTag", 29.0 },
                    { 13, "Aksesoris", "Keyboard wireless slim", "https://placehold.co/603x403", "Apple Keyboard", 99.0 },
                    { 14, "Smartphone", "iPhone murah dengan performa tinggi", "https://placehold.co/603x403", "iPhone SE", 429.0 },
                    { 15, "Tablet", "Tablet kecil dengan kekuatan besar", "https://placehold.co/603x403", "iPad Mini", 499.0 },
                    { 16, "Display", "Layar monitor 5K", "https://placehold.co/603x403", "Apple Studio Display", 1599.0 },
                    { 17, "Desktop", "Desktop powerful untuk kreator", "https://placehold.co/603x403", "Mac Studio", 1999.0 },
                    { 18, "Smartphone", "iPhone flagship", "https://placehold.co/603x403", "iPhone 14 Pro Max", 1199.0 },
                    { 19, "Aksesoris", "Trackpad besar dan responsif", "https://placehold.co/603x403", "Magic Trackpad", 129.0 },
                    { 20, "Wearable", "Smartwatch ekstrem untuk olahraga outdoor", "https://placehold.co/603x403", "Apple Watch Ultra", 799.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}