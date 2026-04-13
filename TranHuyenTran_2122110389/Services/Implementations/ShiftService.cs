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

        public async Task<IEnumerable<Shift>> GetAllAsync()
            => await _context.Shifts
            .Include(s => s.Position)
            .ToListAsync();

        public async Task<Shift> GetByIdAsync(int id)
            => await _context.Shifts.FindAsync(id);

        public async Task<Shift> CreateAsync(Shift model)
        {
            _context.Shifts.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<bool> UpdateAsync(int id, Shift model)
        {
            var existingShift = await _context.Shifts.FindAsync(id);
            if (existingShift == null) return false;

            // Cập nhật các trường
            existingShift.Name = model.Name;
            existingShift.StartTime = model.StartTime;
            existingShift.EndTime = model.EndTime;
            existingShift.PositionId = model.PositionId;
            existingShift.DeptType = model.DeptType;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null) return false;

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}