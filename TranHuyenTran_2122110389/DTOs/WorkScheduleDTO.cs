    using System.ComponentModel.DataAnnotations;

    namespace TranHuyenTran_2122110389.DTOs
    {
        public class WorkScheduleDTO
        {
            public int Id { get; set; }

            // Dùng cho lúc Register (Post)
            public int EmployeeId { get; set; }
            public int ShiftId { get; set; }
            // Dùng cho lúc hiển thị danh sách duyệt (Get)
            public string? EmployeeName { get; set; }
            public string? PositionName { get; set; }
            public string? ShiftName { get; set; }

            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime WorkDate { get; set; }
            public string Status { get; set; } = "Pending";
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }

            public DateTime? CheckInTime { get; set; }
            public DateTime? CheckOutTime { get; set; }
            public string? AttendanceStatus { get; set; }
        }
    }