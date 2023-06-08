using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Profiles
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            //CreateMap<User, MemberDto>()
            //.ForMember(
            //    dest => dest.Age,
            //    options => options.MapFrom(src => src.DateOfBirth.GetAge())
            //);
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
            CreateMap<PaginationDTO<Clinic>, PaginationDTO<ClinicDto>>();
            CreateMap<PaginationDTO<Hospital>, PaginationDTO<HospitalDto>>();
            CreateMap<PaginationDTO<Speciatly>, PaginationDTO<SpeciatlyDto>>();
            CreateMap<PaginationDTO<Schedule>, PaginationDTO<ScheduleDto>>();
            CreateMap<DoctorCreateDto, DoctorDto>();
            CreateMap<Doctor, DoctorDto>();
            CreateMap<PaginationDTO<Doctor>, PaginationDTO<DoctorDto>>();
        }
    }
}
