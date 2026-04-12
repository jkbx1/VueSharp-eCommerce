using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialSupabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    SubCategory = table.Column<string>(type: "text", nullable: false),
                    ItemType = table.Column<string>(type: "text", nullable: false),
                    AvailableSizes = table.Column<string>(type: "text", nullable: false),
                    AvailableColors = table.Column<string>(type: "text", nullable: false),
                    ImageUrls = table.Column<string>(type: "text", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    DefaultAddress = table.Column<string>(type: "text", nullable: false),
                    DefaultCity = table.Column<string>(type: "text", nullable: false),
                    DefaultZip = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Zip = table.Column<string>(type: "text", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SelectedSize = table.Column<string>(type: "text", nullable: false),
                    SelectedColor = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AvailableColors", "AvailableSizes", "Category", "CreatedAt", "Description", "ImageUrls", "ItemType", "Name", "Price", "SKU", "SalePrice", "StockQuantity", "SubCategory" },
                values: new object[,]
                {
                    { 1, "[\"White\",\"Blue\",\"Light Blue\"]", "[\"S\",\"M\",\"L\",\"XL\",\"XXL\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shirt", "Classic Oxford Shirt", 79.99m, "MEN-SHT-001", null, 45, "Shirts" },
                    { 2, "[\"Dark Wash\",\"Black\"]", "[\"30\",\"32\",\"34\",\"36\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1542272604-787c3835535d?w=600\\u0026auto=format\\u0026fit=crop\"]", "Jeans", "Slim Dark Jeans", 119.99m, "MEN-JNS-001", 79.99m, 30, "Jeans" },
                    { 3, "[\"White\",\"Black\",\"Grey\",\"Navy\"]", "[\"XS\",\"S\",\"M\",\"L\",\"XL\",\"XXL\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=600\\u0026auto=format\\u0026fit=crop\"]", "T-Shirt", "Essential T-Shirt", 34.99m, "MEN-TSH-001", null, 120, "T-Shirts" },
                    { 4, "[\"Black\",\"Brown\"]", "[\"S\",\"M\",\"L\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1627123424574-724758594e93?w=600\\u0026auto=format\\u0026fit=crop\"]", "Accessories", "Leather Belt", 45.00m, "MEN-ACC-001", null, 100, "Accessories" },
                    { 5, "[\"Floral\",\"Pink\",\"White\"]", "[\"XS\",\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=600\\u0026auto=format\\u0026fit=crop\"]", "Dress", "Floral Midi Dress", 149.99m, "WOM-DRS-001", 99.99m, 25, "Dresses" },
                    { 6, "[\"Beige\",\"White\",\"Black\"]", "[\"S\",\"M\",\"L\",\"XL\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1594938298603-c8148c4b4af1?w=600\\u0026auto=format\\u0026fit=crop\"]", "Pants", "Linen Trousers", 89.99m, "WOM-PNT-001", null, 15, "Pants" },
                    { 7, "[\"Grey\",\"Beige\",\"Pink\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=600\\u0026auto=format\\u0026fit=crop\"]", "Tops", "Knit Sweater", 109.99m, "WOM-TOP-001", 74.99m, 18, "Tops" },
                    { 8, "[\"Black\",\"Navy\",\"Emerald\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=600\\u0026auto=format\\u0026fit=crop\"]", "Premium", "Silk Velvet Gown", 299.99m, "WOM-PRM-001", null, 5, "Premium Collection" },
                    { 9, "[\"White\",\"Pink\",\"Blue\"]", "[\"0-3M\",\"3-6M\",\"6-9M\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1522771930-78848d9293e8?w=600\\u0026auto=format\\u0026fit=crop\"]", "Onesie", "Baby Organic Onesie", 44.99m, "KID-NWB-001", 29.99m, 60, "Newborn" },
                    { 10, "[\"Pink\",\"White\",\"Gold\"]", "[\"2Y\",\"4Y\",\"6Y\",\"8Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1518831959646-742c3a14ebf7?w=600\\u0026auto=format\\u0026fit=crop\"]", "Dress", "Girls Tulle Dress", 59.99m, "KID-GRL-001", null, 20, "Girl" },
                    { 11, "[\"Blue\",\"Grey\"]", "[\"2Y\",\"4Y\",\"6Y\",\"8Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1544131580-0a187d97b102?w=600\\u0026auto=format\\u0026fit=crop\"]", "Pants", "Boys Denim Joggers", 49.99m, "KID-BOY-001", null, 33, "Boy" },
                    { 12, "[\"Red\",\"Blue\",\"White\"]", "[\"5\",\"6\",\"7\",\"8\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1514989940723-e8e51635b782?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shoes", "Toddler Canvas Shoes", 39.99m, "KID-SHO-001", null, 40, "Shoes" },
                    { 13, "[\"White\",\"Black\",\"Green\"]", "[\"S\",\"M\",\"L\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1622273501267-a220b1283397?w=600\\u0026auto=format\\u0026fit=crop\"]", "T-Shirt", "Clearance Tee", 19.99m, "KID-SAL-001", 9.99m, 100, "Boy" },
                    { 14, "[\"Navy/White\",\"Grey/Black\"]", "[\"S\",\"M\",\"L\",\"XL\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1581655353564-df123a1eb820?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shirt", "Striped Polo Shirt", 55.00m, "MEN-SHT-002", null, 40, "Shirts" },
                    { 15, "[\"Olive\",\"Khaki\",\"Black\"]", "[\"30\",\"32\",\"34\",\"36\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1617114919297-3c8ddb01f599?w=600\\u0026auto=format\\u0026fit=crop\"]", "Jeans", "Cargo Jeans", 89.99m, "MEN-JNS-002", null, 25, "Jeans" },
                    { 16, "[\"White\",\"Navy\",\"Black\"]", "[\"S\",\"M\",\"L\",\"XL\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=600\\u0026auto=format\\u0026fit=crop\"]", "T-Shirt", "Pocket T-Shirt", 24.99m, "MEN-TSH-002", null, 80, "T-Shirts" },
                    { 17, "[\"Grey\",\"Black\",\"Navy\"]", "[\"One Size\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1576871333021-d10ca45a55cc?w=600\\u0026auto=format\\u0026fit=crop\"]", "Accessories", "Beanie Hat", 25.00m, "MEN-ACC-002", null, 60, "Accessories" },
                    { 18, "[\"Black/White\",\"Navy/White\"]", "[\"XS\",\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1496747611176-843222e1e57c?w=600\\u0026auto=format\\u0026fit=crop\"]", "Dress", "Polka Dot Dress", 110.00m, "WOM-DRS-002", null, 30, "Dresses" },
                    { 19, "[\"Beige\",\"Black\",\"Rust\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1594938298603-c8148c4b4af1?w=600\\u0026auto=format\\u0026fit=crop\"]", "Pants", "Wide-Leg Culottes", 95.00m, "WOM-PNT-002", null, 20, "Pants" },
                    { 20, "[\"White\",\"Pink\",\"Lavender\"]", "[\"XS\",\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1583337130417-3346a1be7dee?w=600\\u0026auto=format\\u0026fit=crop\"]", "Tops", "Silk Blouse", 75.00m, "WOM-TOP-002", null, 25, "Tops" },
                    { 21, "[\"Champagne\",\"Navy\",\"Silver\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=600\\u0026auto=format\\u0026fit=crop\"]", "Premium", "Evening Lace Gown", 350.00m, "WOM-PRM-002", null, 8, "Premium Collection" },
                    { 22, "[\"White\",\"Grey\",\"Beige\"]", "[\"0-6M\",\"6-12M\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1522771930-78848d9293e8?w=600\\u0026auto=format\\u0026fit=crop\"]", "Accessories", "Knitted Baby Hat", 15.00m, "KID-NWB-002", null, 50, "Newborn" },
                    { 23, "[\"Pink\",\"Silver\",\"Gold\"]", "[\"2Y\",\"4Y\",\"6Y\",\"8Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1503911216082-ef120f269324?w=600\\u0026auto=format\\u0026fit=crop\"]", "Pants", "Glitter Leggings", 25.00m, "KID-GRL-002", null, 40, "Girl" },
                    { 24, "[\"Black\",\"Grey\",\"Blue\"]", "[\"2Y\",\"4Y\",\"6Y\",\"8Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1622273501267-a220b1283397?w=600\\u0026auto=format\\u0026fit=crop\"]", "T-Shirt", "Graphic Logo Tee", 19.99m, "KID-BOY-002", null, 60, "Boy" },
                    { 25, "[\"Yellow\",\"Blue\",\"Red\"]", "[\"7\",\"8\",\"9\",\"10\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1543163521-1bf539c55dd2?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shoes", "Rubber Rain Boots", 45.00m, "KID-SHO-002", null, 30, "Shoes" },
                    { 26, "[\"Blue\",\"Multicolor\",\"Yellow\"]", "[\"S\",\"M\",\"L\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1519689680058-324335c77eba?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shorts", "Summer Sale Shorts", 29.99m, "KID-SAL-002", 14.99m, 80, "Boy" },
                    { 27, "[\"White\",\"Black\",\"Navy\",\"Olive\"]", "[\"S\",\"M\",\"L\",\"XL\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=600\\u0026auto=format\\u0026fit=crop\"]", "T-Shirt", "Classic Crew Tee", 29.99m, "MEN-TSH-003", null, 100, "T-Shirts" },
                    { 28, "[\"Blue\",\"Light Wash\"]", "[\"S\",\"M\",\"L\",\"XL\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1602810318383-e386cc2a3ccf?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shirt", "Chambray Shirt", 65.00m, "MEN-SHT-003", null, 45, "Shirts" },
                    { 29, "[\"Black\"]", "[\"30\",\"32\",\"34\",\"36\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1541099649105-f69ad21f3246?w=600\\u0026auto=format\\u0026fit=crop\"]", "Jeans", "Black Skinny Jeans", 99.99m, "MEN-JNS-003", null, 35, "Jeans" },
                    { 30, "[\"Brown\",\"Black\",\"Tan\"]", "[\"One Size\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1627123424574-724758594e93?w=600\\u0026auto=format\\u0026fit=crop\"]", "Accessories", "Leather Keyring", 15.00m, "MEN-ACC-003", null, 200, "Accessories" },
                    { 31, "[\"Emerald\",\"Navy\",\"Silver\"]", "[\"XS\",\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1496747611176-843222e1e57c?w=600\\u0026auto=format\\u0026fit=crop\"]", "Dress", "Evening Slip Dress", 130.00m, "WOM-DRS-003", null, 25, "Dresses" },
                    { 32, "[\"White/Navy\",\"Grey/White\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=600\\u0026auto=format\\u0026fit=crop\"]", "Tops", "Striped Knit Top", 45.00m, "WOM-TOP-003", null, 40, "Tops" },
                    { 33, "[\"Black\",\"Navy\",\"Khaki\"]", "[\"S\",\"M\",\"L\",\"XL\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1594938298603-c8148c4b4af1?w=600\\u0026auto=format\\u0026fit=crop\"]", "Pants", "High-Waist Pants", 90.00m, "WOM-PNT-003", null, 15, "Pants" },
                    { 34, "[\"Beige\",\"Camel\",\"Light Grey\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=600\\u0026auto=format\\u0026fit=crop\"]", "Premium", "Cashmere Cardigan", 220.00m, "WOM-PRM-003", null, 10, "Premium Collection" },
                    { 35, "[\"White\",\"Pastel\",\"Mint\"]", "[\"One Size\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1522771930-78848d9293e8?w=600\\u0026auto=format\\u0026fit=crop\"]", "Accessories", "Newborn Swaddle Set", 35.00m, "KID-NWB-003", null, 50, "Newborn" },
                    { 36, "[\"Blue\",\"White\"]", "[\"4Y\",\"6Y\",\"8Y\",\"10Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1503911216082-ef120f269324?w=600\\u0026auto=format\\u0026fit=crop\"]", "Pants", "Denim Skirt", 30.00m, "KID-GRL-003", null, 35, "Girl" },
                    { 37, "[\"Grey\",\"Navy\",\"Black\"]", "[\"4Y\",\"6Y\",\"8Y\",\"10Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1622273501267-a220b1283397?w=600\\u0026auto=format\\u0026fit=crop\"]", "Tops", "Graphic Hoodie", 39.99m, "KID-BOY-003", null, 45, "Boy" },
                    { 38, "[\"Black\",\"Blue\",\"Neon\"]", "[\"1\",\"2\",\"3\",\"4\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1514989940723-e8e51635b782?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shoes", "Running Shoes", 55.00m, "KID-SHO-003", null, 25, "Shoes" },
                    { 39, "[\"Red\",\"Navy\",\"Green\"]", "[\"4Y\",\"6Y\",\"8Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1544131580-0a187d97b102?w=600\\u0026auto=format\\u0026fit=crop\"]", "Outerwear", "Winter Sale Coat", 80.00m, "KID-SAL-003", 40.00m, 15, "Girl" },
                    { 40, "[\"Black\",\"Grey\"]", "[\"S\",\"M\",\"L\",\"XL\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=600\\u0026auto=format\\u0026fit=crop\"]", "T-Shirt", "Graphic Band Tee", 35.00m, "MEN-TSH-004", null, 60, "T-Shirts" },
                    { 41, "[\"Red/Black\",\"Blue/Grey\"]", "[\"S\",\"M\",\"L\",\"XL\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1591047139829-d91aec369bb7?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shirt", "Flannel Shirt", 59.99m, "MEN-SHT-004", null, 40, "Shirts" },
                    { 42, "[\"Indigo\",\"Black\"]", "[\"30\",\"32\",\"34\",\"36\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1542272604-787c3835535d?w=600\\u0026auto=format\\u0026fit=crop\"]", "Jeans", "Relaxed Fit Jeans", 85.00m, "MEN-JNS-004", null, 50, "Jeans" },
                    { 43, "[\"Black\",\"Tortoise\"]", "[\"One Size\"]", "Men", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1511499767350-a151528523f8ae?w=600\\u0026auto=format\\u0026fit=crop\"]", "Accessories", "Sunglasses", 120.00m, "MEN-ACC-004", null, 30, "Accessories" },
                    { 44, "[\"Red\",\"Blue\",\"Black\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1572804013309-59a88b7e92f1?w=600\\u0026auto=format\\u0026fit=crop\"]", "Dress", "Wrap Maxi Dress", 160.00m, "WOM-DRS-004", null, 20, "Dresses" },
                    { 45, "[\"White\",\"Pink\",\"Floral\"]", "[\"XS\",\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1496747611176-843222e1e57c?w=600\\u0026auto=format\\u0026fit=crop\"]", "Tops", "Off-Shoulder Top", 50.00m, "WOM-TOP-004", null, 35, "Tops" },
                    { 46, "[\"Pink\",\"Black\",\"White\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1583337130417-3346a1be7dee?w=600\\u0026auto=format\\u0026fit=crop\"]", "Pants", "Pleated Skirt", 80.00m, "WOM-PNT-004", null, 25, "Pants" },
                    { 47, "[\"Camel\",\"Black\",\"Grey\"]", "[\"S\",\"M\",\"L\"]", "Women", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1591047139829-d91aec369bb7?w=600\\u0026auto=format\\u0026fit=crop\"]", "Premium", "Wool Overcoat", 300.00m, "WOM-PRM-004", null, 5, "Premium Collection" },
                    { 48, "[\"White\",\"Pink\",\"Blue\"]", "[\"0-3M\",\"3-6M\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1522771930-78848d9293e8?w=600\\u0026auto=format\\u0026fit=crop\"]", "Shoes", "Baby Booties", 20.00m, "KID-NWB-004", null, 40, "Newborn" },
                    { 49, "[\"Floral\",\"White\",\"Yellow\"]", "[\"2Y\",\"4Y\",\"6Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1518831959646-742c3a14ebf7?w=600\\u0026auto=format\\u0026fit=crop\"]", "Dress", "Summer Sundress", 35.00m, "KID-GRL-004", null, 25, "Girl" },
                    { 50, "[\"Yellow\",\"Blue\"]", "[\"4Y\",\"6Y\",\"8Y\"]", "Kids", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "", "[\"https://images.unsplash.com/photo-1544131580-0a187d97b102?w=600\\u0026auto=format\\u0026fit=crop\"]", "Outerwear", "Raincoat", 45.00m, "KID-BOY-004", null, 15, "Boy" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
