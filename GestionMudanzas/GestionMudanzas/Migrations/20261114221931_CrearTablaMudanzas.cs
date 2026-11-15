using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionMudanzas.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaMudanzas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mudanzas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    VehiculoId = table.Column<int>(type: "int", nullable: false),
                    FechaServicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DireccionOrigen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DireccionDestino = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mudanzas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mudanzas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mudanzas_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mudanzas_ClienteId",
                table: "Mudanzas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Mudanzas_VehiculoId",
                table: "Mudanzas",
                column: "VehiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mudanzas");
        }
    }
}
