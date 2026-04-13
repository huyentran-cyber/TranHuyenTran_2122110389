namespace TranHuyenTran_2122110389.DTOs
{
    public class SalaryDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalDays { get; set; }
        public decimal TotalHours { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal PenaltyViolation { get; set; } // Phạt đi muộn/về sớm
        public decimal PenaltyAbsent { get; set; }    // Phạt nghỉ không phép
        public decimal TotalAmount { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}