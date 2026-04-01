namespace TranHuyenTran_2122110389.Models
{
    public class Position
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal HourlyRate { get; set; }

        public int MinStaff { get; set; }

        public int MaxShiftPerDay { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}