namespace TranHuyenTran_2122110389.DTOs
{
    public class ShiftDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}