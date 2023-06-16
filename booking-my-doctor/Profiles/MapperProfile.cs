using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Appointment;
using booking_my_doctor.DTOs.Hospital;

namespace booking_my_doctor.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterUserDto, User>();
            CreateMap<UserCreateDto, User>();
            CreateMap<User, UserDTO>().ForMember(dest => dest.roleName, opt => opt.MapFrom(src => src.role.Name)); ;
            CreateMap<UserUpdateDTO, User>();
            CreateMap<User, UserUpdateDTO>();
            CreateMap<Clinic, ClinicDto>();
            CreateMap<Hospital, HospitalDto>();
            CreateMap<Speciatly, SpeciatlyDto>();
            CreateMap<ClinicDto, Clinic>();
            CreateMap<HospitalDto, Hospital>();
            CreateMap<SpeciatlyDto, Speciatly>();
            CreateMap<Schedule, ScheduleDto>();
            CreateMap<ScheduleDto, Schedule>();
            CreateMap<Schedule, ScheduleView>().ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.user.fullName));
            CreateMap<PaginationDTO<Clinic>, PaginationDTO<ClinicDto>>();
            CreateMap<PaginationDTO<Hospital>, PaginationDTO<HospitalDto>>();
            CreateMap<PaginationDTO<Speciatly>, PaginationDTO<SpeciatlyDto>>();
            CreateMap<PaginationDTO<Schedule>, PaginationDTO<ScheduleDto>>();
            CreateMap<ScheduleCreateDto, Schedule>();
            CreateMap<DoctorCreateDto, DoctorDto>();
            CreateMap<Doctor, DoctorDto>();
            CreateMap<PaginationDTO<Doctor>, PaginationDTO<DoctorDto>>();
            CreateMap<Appointment, AppointmentView>();
            CreateMap<PaginationDTO<Appointment>, PaginationDTO<AppointmentView>>();
            CreateMap<AppointmentCreate, Appointment>();
            CreateMap<List<Doctor>, DoctorDto>();
            CreateMap<Hospital, HospitalDetail>();

        }
    }
}
