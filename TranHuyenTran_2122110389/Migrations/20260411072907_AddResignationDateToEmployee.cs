using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranHuyenTran_2122110389.Migrations
{
    /// <inheritdoc />
    public partial class AddResignationDateToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResignationDate",
                table: "Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Password", "ResignationDate" },
                values: new object[] { "$2a$11$AlL8i.4vq.mcMBcuX2JZOuDTn6lkC9SXzKzd8OoeofGqmllL6LYXW", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResignationDate",
                table: "Employees");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$y5G/xFlE7dFUShihK.w4Rurz0LoCBEB1qpTtm8elgHu5WHDZwe/dS");
        }
    }
}
