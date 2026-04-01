namespace TranHuyenTran_2122110389.Models
{
    public class WorkSchedule
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public int ShiftId { get; set; }

        public Shift? Shift { get; set; }

        public DateTime WorkDate { get; set; }
    }
}