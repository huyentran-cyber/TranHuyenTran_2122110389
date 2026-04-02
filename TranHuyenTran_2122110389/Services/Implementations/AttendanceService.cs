using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _context;

        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Attendance> GetAll()
        {
            return _context.Attendances.Include(x => x.Employee);
        }

        public Attendance CheckIn(int employeeId)
        {
            var today = DateTime.Today;

            var exist = _context.Attendances
                .FirstOrDefault(x => x.EmployeeId == employeeId && x.CheckIn.Date == today);

            if (exist != null)
                throw new Exception("Already checked in");

            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                CheckIn = DateTime.Now,
                Status = "Present"
            };

            _context.Attendances.Add(attendance);
            _context.SaveChanges();

            return attendance;
        }

        public Attendance CheckOut(int employeeId)
        {
            var today = DateTime.Today;

            var attendance = _context.Attendances
                .FirstOrDefault(x => x.EmployeeId == employeeId && x.CheckIn.Date == today);

            if (attendance == null)
                throw new Exception("Not checked in");

            attendance.CheckOut = DateTime.Now;

            _context.SaveChanges();

            return attendance;
        }
    }
}
