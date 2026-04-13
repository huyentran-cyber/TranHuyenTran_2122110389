namespace TranHuyenTran_2122110389.DTOs
{
    public class SalaryReportDTO
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string PositionName { get; set; }

        public int TotalDays { get; set; }
        public string? Violations { get; set; }
        public int TotalShifts { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public decimal HourlyRate { get; set; }
        public decimal TotalHours { get; set; }

        public decimal TotalSalary { get; set; }
        public decimal PenaltyAbsent { get; set; }
        public decimal PenaltyViolation { get; set; }
        public decimal TotalPenalty { get; set; }
    }
}