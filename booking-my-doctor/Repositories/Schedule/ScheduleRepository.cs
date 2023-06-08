﻿using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly MyDbContext _context;

        public ScheduleRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationDTO<ScheduleView>> GetSchedules(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null, DateTime? date = null, string? sortColumn = "StartTime")
        {
            var query = _context.Schedules.AsQueryable();

            if (doctorId != null)
            {
                query = query.Where(s => s.DoctorId == doctorId);
            }
            if (date != null)
            {
                query = query.Where(s => s.StartTime.Date == date.Value.Date);
            }

            switch (sortColumn)
            {
                case "Id":
                    query = query.OrderBy(u => u.Id);
                    break;
                case "StartTime":
                    query = query.OrderByDescending(u => u.StartTime);
                    break;
                default:
                    query = query.OrderBy(u => u.Id);
                    break;
            }
            var pagination = new PaginationDTO<ScheduleView>();
            var schedules = new List<Schedule>();

            schedules = await query.ToListAsync();
            pagination.TotalCount = schedules.Count;

            var schedulesView = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).Select(s => new ScheduleView
            {
                Id = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Cost = s.Cost,
                Status = s.Status,
                DoctorId = s.DoctorId,
                DoctorName = s.Doctor.user.fullName
            }).ToListAsync();
            pagination.PageSize = pageSize.Value;
            pagination.Page = page.Value;
            pagination.ListItem = schedulesView;
            return pagination;
        }
        public async Task<Schedule> GetScheduleById(int id)
        {
            return await _context.Schedules.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> IsSaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateSchedule(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            return true;
        }

        public async Task<bool> UpdateSchedule(Schedule schedule)
        {
            _context.Entry(schedule).State = EntityState.Modified;
            return true;
        }

        public async Task<bool> DeleteSchedule(Schedule schedule)
        {
            _context.Remove(schedule);
            return true;
        }
    }
}