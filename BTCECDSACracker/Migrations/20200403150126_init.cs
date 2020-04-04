using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BTCECDSACracker.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeightLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WeightsHL0 = table.Column<string>(nullable: true),
                    WeightsHL1 = table.Column<string>(nullable: true),
                    WeightsHL2 = table.Column<string>(nullable: true),
                    WeightsOL = table.Column<string>(nullable: true),
                    Byte0 = table.Column<int>(nullable: false),
                    Byte1 = table.Column<int>(nullable: false),
                    Byte2 = table.Column<int>(nullable: false),
                    Byte3 = table.Column<int>(nullable: false),
                    Byte4 = table.Column<int>(nullable: false),
                    Byte5 = table.Column<int>(nullable: false),
                    Byte6 = table.Column<int>(nullable: false),
                    Byte7 = table.Column<int>(nullable: false),
                    Byte8 = table.Column<int>(nullable: false),
                    Byte9 = table.Column<int>(nullable: false),
                    Byte10 = table.Column<int>(nullable: false),
                    Byte11 = table.Column<int>(nullable: false),
                    Byte12 = table.Column<int>(nullable: false),
                    Byte13 = table.Column<int>(nullable: false),
                    Byte14 = table.Column<int>(nullable: false),
                    Byte15 = table.Column<int>(nullable: false),
                    Byte16 = table.Column<int>(nullable: false),
                    Byte17 = table.Column<int>(nullable: false),
                    Byte18 = table.Column<int>(nullable: false),
                    Byte19 = table.Column<int>(nullable: false),
                    Byte20 = table.Column<int>(nullable: false),
                    Byte21 = table.Column<int>(nullable: false),
                    Byte22 = table.Column<int>(nullable: false),
                    Byte23 = table.Column<int>(nullable: false),
                    Byte24 = table.Column<int>(nullable: false),
                    Byte25 = table.Column<int>(nullable: false),
                    Byte26 = table.Column<int>(nullable: false),
                    Byte27 = table.Column<int>(nullable: false),
                    Byte28 = table.Column<int>(nullable: false),
                    Byte29 = table.Column<int>(nullable: false),
                    Byte30 = table.Column<int>(nullable: false),
                    Byte31 = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightLogs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeightLogs");
        }
    }
}
