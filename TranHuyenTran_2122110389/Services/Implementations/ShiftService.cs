using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class ShiftService : IShiftService
    {
        private readonly AppDbContext _context;

        public ShiftService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Shift> GetAll()
        {
            return _context.Shifts;
        }

        public Shift Create(Shift model)
        {
            _context.Shifts.Add(model);
            _context.SaveChanges();
            return model;
        }
    }
}
