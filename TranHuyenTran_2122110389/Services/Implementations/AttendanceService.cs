using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _context;

        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        // Sửa kiểu trả về thành IEnumerable để khớp với Interface
        public IEnumerable<Attendance> GetAll()
        {
            return _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending(a => a.CheckIn)
                .ToList();
        }

        public Attendance CheckIn(int employeeId)
        {
            var today = DateTime.Today;
            var existing = _context.Attendances
                .FirstOrDefault(a => a.EmployeeId == employeeId && a.CheckIn.Date == today);

            if (existing != null)
                throw new Exception("Hôm nay bạn đã Check-in rồi.");

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
            // Tìm bản ghi check-in của hôm nay mà chưa có giờ check-out
            var attendance = _context.Attendances
                .FirstOrDefault(a => a.EmployeeId == employeeId && a.CheckIn.Date == today && a.CheckOut == null);

            if (attendance == null)
                throw new Exception("Không tìm thấy dữ liệu Check-in hợp lệ để Check-out.");

            attendance.CheckOut = DateTime.Now;
            _context.SaveChanges();
            return attendance;
        }
    }
}