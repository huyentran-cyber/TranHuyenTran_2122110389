namespace TranHuyenTran_2122110389.DTOs
{
    public class PositionDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal HourlyRate { get; set; }

        public int MinStaff { get; set; }

        public int MaxShiftPerDay { get; set; }
    }
}