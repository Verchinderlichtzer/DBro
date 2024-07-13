using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DBro.API.Migrations
{
    /// <inheritdoc />
    public partial class First : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Kategori = table.Column<byte>(type: "tinyint", nullable: false),
                    Nama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Harga = table.Column<int>(type: "int", nullable: false),
                    Gambar = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JenisKelamin = table.Column<byte>(type: "tinyint", nullable: false),
                    TanggalLahir = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Alamat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telepon = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    JenisUser = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "Diskon",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdMenu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nilai = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    TanggalMulai = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TanggalAkhir = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diskon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diskon_Menu_IdMenu",
                        column: x => x.IdMenu,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Promo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdMenuDibeli = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdMenuDidapat = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JumlahDibeli = table.Column<int>(type: "int", nullable: false),
                    JumlahDidapat = table.Column<int>(type: "int", nullable: false),
                    TanggalMulai = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TanggalAkhir = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promo_Menu_IdMenuDibeli",
                        column: x => x.IdMenuDibeli,
                        principalTable: "Menu",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Promo_Menu_IdMenuDidapat",
                        column: x => x.IdMenuDidapat,
                        principalTable: "Menu",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Aktivitas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Jenis = table.Column<byte>(type: "tinyint", nullable: false),
                    Entitas = table.Column<byte>(type: "tinyint", nullable: false),
                    IdEntitas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aktivitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aktivitas_User_Email",
                        column: x => x.Email,
                        principalTable: "User",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pesanan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Subtotal = table.Column<int>(type: "int", nullable: false),
                    Potongan = table.Column<int>(type: "int", nullable: false),
                    Bayar = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pesanan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pesanan_User_Email",
                        column: x => x.Email,
                        principalTable: "User",
                        principalColumn: "Email");
                });

            migrationBuilder.CreateTable(
                name: "DetailPesanan",
                columns: table => new
                {
                    IdPesanan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdMenu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Harga = table.Column<int>(type: "int", nullable: false),
                    Jumlah = table.Column<int>(type: "int", nullable: false),
                    Diskon = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailPesanan", x => new { x.IdPesanan, x.IdMenu });
                    table.ForeignKey(
                        name: "FK_DetailPesanan_Menu_IdMenu",
                        column: x => x.IdMenu,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetailPesanan_Pesanan_IdPesanan",
                        column: x => x.IdPesanan,
                        principalTable: "Pesanan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuPromoPesanan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPesanan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdMenu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Jumlah = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuPromoPesanan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuPromoPesanan_Menu_IdMenu",
                        column: x => x.IdMenu,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuPromoPesanan_Pesanan_IdPesanan",
                        column: x => x.IdPesanan,
                        principalTable: "Pesanan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "Gambar", "Harga", "Kategori", "Nama" },
                values: new object[,]
                {
                    { "M-0001", null, 7000, (byte)1, "Sayap" },
                    { "M-0002", null, 8000, (byte)1, "Paha Bawah" },
                    { "M-0003", null, 10000, (byte)1, "Dada" },
                    { "M-0004", null, 10000, (byte)1, "Paha Atas" },
                    { "M-0005", null, 10500, (byte)2, "DBRO 1 Sayap + Nasi" },
                    { "M-0006", null, 11500, (byte)2, "DBRO 2 Paha Bawah + Nasi" },
                    { "M-0007", null, 13000, (byte)2, "DBRO 3 Dada / Paha Atas + Nasi" },
                    { "M-0008", null, 10000, (byte)3, "Spaghetti" },
                    { "M-0009", null, 8000, (byte)4, "DBRO Burger" },
                    { "M-0010", null, 10500, (byte)4, "DBRO Reguler" },
                    { "M-0011", null, 12000, (byte)4, "DBRO Chiz" },
                    { "M-0012", null, 13000, (byte)4, "DBRO Premium" },
                    { "M-0013", null, 15000, (byte)4, "DBRO Premium Chiz" },
                    { "M-0014", null, 9000, (byte)5, "DBROsis" },
                    { "M-0015", null, 17000, (byte)5, "DBROsis + Kentang + Minum*" },
                    { "M-0016", null, 13000, (byte)6, "Sayhot Dada" },
                    { "M-0017", null, 10000, (byte)6, "Sayhot Sayap" },
                    { "M-0018", null, 11000, (byte)6, "Sayhot Paha Bawah" },
                    { "M-0019", null, 21000, (byte)7, "Tteokbokki Reguler" },
                    { "M-0020", null, 30000, (byte)7, "Tteokbokki Medium" },
                    { "M-0021", null, 40000, (byte)7, "Tteokbokki Large" },
                    { "M-0022", null, 11000, (byte)8, "Sayap Sambal Merah" },
                    { "M-0023", null, 12000, (byte)8, "Paha Bawah Sambal Merah" },
                    { "M-0024", null, 13000, (byte)1, "Kulit" },
                    { "M-0025", null, 3000, (byte)9, "Prima" },
                    { "M-0026", null, 4000, (byte)9, "Teh Botol Sosro" },
                    { "M-0027", null, 4000, (byte)9, "S-tee" },
                    { "M-0028", null, 5500, (byte)9, "Nestle Orange" },
                    { "M-0029", null, 6000, (byte)9, "Milo" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Email", "Alamat", "JenisKelamin", "JenisUser", "Nama", "Password", "TanggalLahir", "Telepon" },
                values: new object[] { "sujudihanif36@gmail.com", "Perumahan Bumi Anggrek Blok K No 80", (byte)1, (byte)0, "Sujudi Hanif", "IvCkErOjG9A8DPW7X23rJg==", new DateTime(2002, 6, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "085739194810" });

            migrationBuilder.CreateIndex(
                name: "IX_Aktivitas_Email",
                table: "Aktivitas",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_DetailPesanan_IdMenu",
                table: "DetailPesanan",
                column: "IdMenu");

            migrationBuilder.CreateIndex(
                name: "IX_Diskon_IdMenu",
                table: "Diskon",
                column: "IdMenu");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPromoPesanan_IdMenu",
                table: "MenuPromoPesanan",
                column: "IdMenu");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPromoPesanan_IdPesanan",
                table: "MenuPromoPesanan",
                column: "IdPesanan");

            migrationBuilder.CreateIndex(
                name: "IX_Pesanan_Email",
                table: "Pesanan",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Promo_IdMenuDibeli",
                table: "Promo",
                column: "IdMenuDibeli");

            migrationBuilder.CreateIndex(
                name: "IX_Promo_IdMenuDidapat",
                table: "Promo",
                column: "IdMenuDidapat");

            migrationBuilder.CreateIndex(
                name: "IX_User_Telepon",
                table: "User",
                column: "Telepon",
                unique: true,
                filter: "[Telepon] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aktivitas");

            migrationBuilder.DropTable(
                name: "DetailPesanan");

            migrationBuilder.DropTable(
                name: "Diskon");

            migrationBuilder.DropTable(
                name: "MenuPromoPesanan");

            migrationBuilder.DropTable(
                name: "Promo");

            migrationBuilder.DropTable(
                name: "Pesanan");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
