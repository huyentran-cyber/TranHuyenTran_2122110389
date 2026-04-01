namespace TranHuyenTran_2122110389.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public Role Role { get; set; } = Role.Staff;

        public int PositionId { get; set; }

        public Position? Position { get; set; }

        public ICollection<WorkSchedule>? WorkSchedules { get; set; }

        public ICollection<Attendance>? Attendances { get; set; }
    }
}   