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
            => await _context.Shifts.ToListAsync();

        public async Task<Shift> GetByIdAsync(int id)
            => await _context.Shifts.FindAsync(id);

        public async Task<Shift> CreateAsync(Shift model)
        {
            _context.Shifts.Add(model);
            await _context.SaveChangesAsync();
            return model;
        }

    }
}