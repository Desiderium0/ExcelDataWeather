using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.Migrations
{
    /// <inheritdoc />
    public partial class WeatherData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Time = table.Column<TimeSpan>(type: "time", nullable: true),
                    T = table.Column<double>(type: "float", nullable: true),
                    RelativeHumidity = table.Column<int>(type: "int", nullable: true),
                    Td = table.Column<double>(type: "float", nullable: true),
                    AtmosphericPressure = table.Column<double>(type: "float", nullable: true),
                    WindDirection = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WindSpeed = table.Column<double>(type: "float", nullable: true),
                    Cloudiness = table.Column<int>(type: "int", nullable: true),
                    h = table.Column<int>(type: "int", nullable: true),
                    VV = table.Column<double>(type: "float", nullable: true),
                    WeatherPhenomena = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherData");
        }
    }
}
