namespace TranHuyenTran_2122110389.DTOs
{
    public class LeaveRequestDTO
    {
        public int EmployeeId { get; set; }
        public DateTime OffDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
