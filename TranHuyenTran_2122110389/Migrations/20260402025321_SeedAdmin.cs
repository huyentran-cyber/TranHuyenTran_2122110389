using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TranHuyenTran_2122110389.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "HourlyRate", "MaxShiftPerDay", "MinStaff", "Name" },
                values: new object[,]
                {
                    { 1, 0m, 1, 1, "Admin" },
                    { 2, 100m, 2, 1, "Staff" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Email", "Name", "Password", "Phone", "PositionId", "Role" },
                values: new object[] { 1, "admin@gmail.com", "Admin", "$2a$11$w3xYVLpMugYIHz.O9kkjzumflalptf4xDnf3UcUp1fK3QP7qILw5i", "0000000000", 1, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
