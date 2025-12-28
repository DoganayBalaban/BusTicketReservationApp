using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // İlk şema: seferler ve rezervasyonlar tablolarını ilişkileriyle kur
            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Company = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DepartureCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ArrivalCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    BusType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Seats = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TripId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PassengerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PassengerSurname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TCKimlik = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Seats = table.Column<string>(type: "text", nullable: false),
                    ReservationCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationCode",
                table: "Reservations",
                column: "ReservationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_TripId",
                table: "Reservations",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ArrivalCity",
                table: "Trips",
                column: "ArrivalCity");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DepartureCity",
                table: "Trips",
                column: "DepartureCity");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_DepartureTime",
                table: "Trips",
                column: "DepartureTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
