using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TranHuyenTran_2122110389.Migrations
{
    /// <inheritdoc />
    public partial class AddPositionToShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Thêm cột PositionId (tạm thời để nullable: true để không bị ép giá trị 0 ngay lập tức)
            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "Shifts",
                type: "int",
                nullable: true); // Chỉnh lại thành true để vượt qua bước kiểm tra ban đầu

            // 2. Cập nhật mật khẩu Admin (giữ nguyên của bạn)
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$y5G/xFlE7dFUShihK.w4Rurz0LoCBEB1qpTtm8elgHu5WHDZwe/dS");

            // 3. QUAN TRỌNG: Gán PositionId cho các ca làm việc cũ.
            // Giả sử trong bảng Positions bạn có Id = 1 là "Pha chế", Id = 2 là "Phục vụ"
            // Ta gán tất cả ca cũ về Id = 1 (hoặc Id nào bạn CHẮC CHẮN đang tồn tại trong bảng Positions)
            migrationBuilder.Sql("UPDATE Shifts SET PositionId = 1");

            // 4. Bây giờ mới chuyển cột PositionId về lại nullable: false (bắt buộc)
            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "Shifts",
                type: "int",
                nullable: false);

            // 5. Tạo Index và Khóa ngoại (Giữ nguyên hoặc chỉnh thành Restrict/NoAction)
            migrationBuilder.CreateIndex(
                name: "IX_Shifts_PositionId",
                table: "Shifts",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Positions_PositionId",
                table: "Shifts",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict); // Dùng Restrict để tránh lỗi Cycle
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Positions_PositionId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_PositionId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Shifts");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$iPtWKjfNwq3PV.dzDDfzW.f78gtKLKuOW.jhzPaXDYecq9Vlr7rI.");
        }
    }
}
