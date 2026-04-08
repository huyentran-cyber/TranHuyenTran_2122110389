using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class ShiftService : IShiftService
    {
        private readonly AppDbContext _context;
        public ShiftService(AppDbContext context) => _context = context;

        public IEnumerable<Shift> GetAll() => _context.Shifts.ToList();

        public Shift Create(Shift model)
        {
            _context.Shifts.Add(model);
            _context.SaveChanges();
            return model;
        }

        // THỰC THI CÁC HÀM MỚI
        public int GetShiftCountByDate(int employeeId, DateTime date)
        {
            return _context.WorkSchedules
                .Count(ws => ws.EmployeeId == employeeId && ws.Date.Date == date.Date);
        }

        public WorkSchedule Register(WorkScheduleDTO dto)
        {
            var schedule = new WorkSchedule
            {
                EmployeeId = dto.EmployeeId,
                ShiftId = dto.ShiftId,
                Date = dto.Date
            };
            _context.WorkSchedules.Add(schedule);
            _context.SaveChanges();
            return schedule;
        }

        public IEnumerable<WorkSchedule> GetByEmployeeId(int employeeId)
        {
            return _context.WorkSchedules
                .Where(ws => ws.EmployeeId == employeeId)
                .Include(ws => ws.Shift)
                .ToList();
        }
    }
}