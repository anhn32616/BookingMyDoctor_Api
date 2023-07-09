using AutoMapper;
using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Appointment;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Repositories
{
    public class StatisticalRepository : IStatisticalRepository
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;

        public StatisticalRepository(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<QuantityStatistics> GetQuantityStatistics()
        {
            return new QuantityStatistics
            {
                CountClinic = await _context.Clinics.CountAsync(),
                CountDoctor = await _context.Doctors.CountAsync(),
                CountHospital = await _context.Hospitals.CountAsync(),
                CountPatient = await _context.Users.Where(u => u.role.Name == "ROLE_PATIENT").CountAsync()
            };
        }

        public async Task<Statistical> GetStatistical(DateTime startTime, DateTime endTime, int? page = null, int? pageSize = null, int? dId = null)
        {
            var query = (from d in _context.Doctors
                         join s in _context.Schedules on d.Id equals s.DoctorId
                         join a in _context.Appointments on s.Id equals a.ScheduleId
                         where a.Status == "Done" && a.date.Date >= startTime.Date && a.date.Date <= endTime.Date
                         let doctorId = d.Id
                         let totalAppointmentDone = _context.Appointments.Where(a => (a.Schedule.DoctorId == doctorId) && (a.Status == "Done") && a.date.Date >= startTime.Date && a.date <= endTime.Date).Count()
                         let revenue = _context.Appointments.Where(a => (a.Schedule.DoctorId == doctorId) && (a.Status == "Done") && a.date.Date >= startTime.Date && a.date.Date <= endTime.Date).Sum(a => a.Schedule.Cost)
                         let feePaid = _context.Appointments.Where(a => (a.Schedule.DoctorId == doctorId) && (a.Status == "Done") && a.date.Date >= startTime.Date && a.date.Date <= endTime.Date && a.Paid == true).Sum(a => a.Schedule.Cost)
                         select new DoctorRevenue
                         {
                             DoctorId = doctorId,
                             Email = d.user.email,
                             PhoneNumber = d.user.phoneNumber,
                             FullName = d.user.fullName,
                             TotalAppointmentDone = totalAppointmentDone,
                             Revenue = revenue,
                             Profit = revenue * 90 / 100,
                             Fee = revenue * 10 / 100,
                             FeePaid = feePaid * 10 / 100
                         }).Distinct().AsQueryable();
            var pagination = new PaginationDTO<DoctorRevenue>();
            var doctorRevenues = new List<DoctorRevenue>();

            doctorRevenues = await query.ToListAsync();
            var totalMonthlyFee = await _context.Payments.Where(p => p.DatePayment.Date >= startTime.Date && p.DatePayment.Date <= endTime.Date && p.Status == true).SumAsync(p => p.MonthlyFee);
            var result = new Statistical
            {
                CompanyRevenue = doctorRevenues.Sum(dr => dr.FeePaid) + totalMonthlyFee,
                TotalMonthlyFee = totalMonthlyFee,
                DoctorRevenue = doctorRevenues.Sum(dr => dr.Revenue),
                DoctorProfit = doctorRevenues.Sum(dr => dr.Profit),
                TotalAppointmentDone = doctorRevenues.Sum(dr => dr.TotalAppointmentDone),
                TotalDoctorActivity = doctorRevenues.Count(),
                FeeAppointment = doctorRevenues.Sum(dr => dr.Fee),
                FeeAppointmentPaid = doctorRevenues.Sum(dr => dr.FeePaid),
                PaginationDoctorRevenues = pagination
            };
            if (dId != null)
            {
                doctorRevenues = doctorRevenues.Where(d => d.DoctorId == dId).ToList();
            }
            pagination.TotalCount = doctorRevenues.Count;
            if (page == null || pageSize == null)
            {
                pagination.Page = 0;
                pagination.PageSize = (pagination.TotalCount != 0) ? pagination.TotalCount : 10;
            }
            else
            {
                doctorRevenues = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).ToListAsync();
                pagination.PageSize = pageSize.Value;
                pagination.Page = page.Value;
            }
            pagination.ListItem = doctorRevenues;
            result.PaginationDoctorRevenues = pagination;
            return result;
        }

        public async Task<DoctorRevenue> GetStatisticsOfDoctor(int id, DateTime startTime, DateTime endTime)
        {
            var res = await (from d in _context.Doctors
                             where d.Id == id
                             join s in _context.Schedules on d.Id equals s.DoctorId
                             join a in _context.Appointments on s.Id equals a.ScheduleId
                             where a.Status == "Done" && a.date.Date >= startTime.Date && a.date.Date <= endTime.Date
                             let doctorId = d.Id
                             let totalAppointmentDone = _context.Appointments.Where(a => (a.Schedule.DoctorId == doctorId) && (a.Status == "Done") && a.date.Date >= startTime.Date && a.date <= endTime.Date).Count()
                             let revenue = _context.Appointments.Where(a => (a.Schedule.DoctorId == doctorId) && (a.Status == "Done") && a.date.Date >= startTime.Date && a.date.Date <= endTime.Date).Sum(a => a.Schedule.Cost)
                             let feePaid = _context.Appointments.Where(a => (a.Schedule.DoctorId == doctorId) && (a.Status == "Done") && a.date.Date >= startTime.Date && a.date.Date <= endTime.Date && a.Paid == true).Sum(a => a.Schedule.Cost)
                             select new DoctorRevenue
                             {
                                 DoctorId = doctorId,
                                 Email = d.user.email,
                                 PhoneNumber = d.user.phoneNumber,
                                 FullName = d.user.fullName,
                                 TotalAppointmentDone = totalAppointmentDone,
                                 Revenue = revenue,
                                 Profit = revenue * 90 / 100,
                                 Fee = revenue * 10 / 100,
                                 FeePaid = feePaid * 10 / 100,
                                 Appointments = new List<AppointmentView>()
                             }).FirstOrDefaultAsync();
            if (res == null) return new DoctorRevenue
            {
                DoctorId = id,
                Email = "",
                PhoneNumber = "",
                FullName = "",
                TotalAppointmentDone = 0,
                Revenue = 0,
                Profit = 0,
                Fee = 0,
                FeePaid = 0,
                Appointments = new List<AppointmentView>()
            };
            var appointments = await _context.Appointments.Include(a => a.Schedule).Include(a => a.Schedule.Doctor.user).Include(a => a.Patient).Where(a => (a.Schedule.DoctorId == id) && (a.Status == "Done") && a.date.Date >= startTime.Date && a.date.Date <= endTime.Date).OrderByDescending(a => a.date).ToListAsync();
            var appointmentsView = new List<AppointmentView>();
            if (appointments != null) appointmentsView = appointments.Select(_mapper.Map<Appointment, AppointmentView>).ToList();
            res.Appointments = appointmentsView;
            return res;
        }
    }
}
