namespace TranHuyenTran_2122110389.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public string? Status { get; set; }
    }
}   