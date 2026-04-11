using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranHuyenTran_2122110389.Migrations
{
    /// <inheritdoc />
    public partial class AddHourlyRateToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "HourlyRate",
                table: "Employees",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "HourlyRate", "Password" },
                values: new object[] { 0m, "$2a$11$8.8giRVEnNInv2OGXMKDFePVUhvkv5jXjIPVc6YQodw/roadNaSH." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourlyRate",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$cKSkJozZDgiUzhkPL6CJeODJXrHDXPYQ1Ntc1lHTBkimTHOqmRRwC");
        }
    }
}
