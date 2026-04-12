namespace TranHuyenTran_2122110389.DTOs
{
    public class AttendanceDTO
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? PositionName { get; set; }
        public string? ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public string? Status { get; set; }
    }
}