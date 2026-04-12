namespace TranHuyenTran_2122110389.DTOs
{
    public class LeaveRequestDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime OffDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsEmergency { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
