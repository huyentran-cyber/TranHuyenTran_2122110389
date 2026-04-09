namespace TranHuyenTran_2122110389.DTOs
{
    public class SalaryReportDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string PositionName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalHours { get; set; }
        public decimal TotalAmount { get; set; }
    }
}