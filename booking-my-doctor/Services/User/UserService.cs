﻿using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Repositories;
using MyWebApiApp.Models;
using System.Security.Cryptography;
using System.Text;

namespace booking_my_doctor.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        public UserService(IUserRepository userRepository,
            IMapper mapper,
            IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }
        public async Task<ApiResponse> CreateUser(UserCreateDto userDto)
        {
            try
            {
                // Kiem tra email da ton tai hay chua
                var check = _userRepository.IsEmailAlreadyExists(userDto.email);
                var roleUser = _roleRepository.getRoleByName(userDto.roleName);
                Task.WaitAll(check, roleUser);

                if (check.Result)
                {
                    return new ApiResponse
                    {
                        statusCode = 400,
                        message = "Email đã tồn tại"
                    };
                }
                if (roleUser.Result == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Roles không tồn tại"
                    };
                }

                //Tao acc va return token
                if (userDto.countViolation == null) userDto.countViolation = 0;
                using var hmac = new HMACSHA512();
                var passwordBytes = Encoding.UTF8.GetBytes("Abcd123@");
                var newUser = _mapper.Map<UserCreateDto, User>(userDto);
                if (newUser.image == null)
                {
                    newUser.image = newUser.gender == false ? "https://cdn4.iconfinder.com/data/icons/medical-and-health-15/128/115-512.png"
                         : "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSetlcP-zX0tDv47hUaZ2Z3e_QKYclz3XzSOm9CJPlsqjYOWDDDP8dvk0wQZ3DQ6Ybyae4&usqp=CAU";
                }

                newUser.isDelete = false;
                newUser.isEmailVerified = true;
                newUser.roleId = roleUser.Result.Id;
                newUser.PasswordSalt = hmac.Key;
                newUser.PasswordHash = hmac.ComputeHash(passwordBytes);
                await _userRepository.CreateUser(newUser);
                await _userRepository.IsSaveChanges();


                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }

        }

        public async Task<ApiResponse> DeleteUser(int userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return new ApiResponse
                {
                    statusCode = 404,
                    message = "Không tồn tại user có id này"
                };
            }
            await _userRepository.DeleteUser(user);
            await _userRepository.IsSaveChanges();
            return new ApiResponse
            {
                statusCode = 200,
                message = "Success"
            };
        }

        public async Task<ApiResponse> GetAllUsers(int? page, int? pageSize, string? name, string? sortColumn, string? roleName)
        {
            try
            {
                var pagination = await _userRepository.GetUsers(page, pageSize, name, sortColumn, roleName);
                var paginationUserDTO = new PaginationDTO<UserDTO>();
                paginationUserDTO.PageSize = pagination.PageSize;
                paginationUserDTO.TotalCount = pagination.TotalCount;
                paginationUserDTO.Page = pagination.Page;

                if (pagination.ListItem != null)
                {
                    paginationUserDTO.ListItem = pagination.ListItem.Select(_mapper.Map<User, UserDTO>).ToList();
                }
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success",
                    data = paginationUserDTO
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }

        }
        public async Task<ApiResponse> GetUserById(int? id)
        {
            try
            {
                if (id == null) return new ApiResponse
                {
                    statusCode = 400,
                    message = "Thiếu tham số id user"
                };
                var result = await _userRepository.GetUserById(id.Value);
                if (result == null) return new ApiResponse
                {
                    statusCode = 404,
                    message = $"Không tìm thấy user có id {id.Value}"
                };
                var user = _mapper.Map<UserDTO>(result);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success",
                    data = user
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }

        }

        public async Task<ApiResponse> UpdateUser(int userId, UserUpdateDTO userUpdateDTO)
        {
            try
            {
                var userCurrent = await _userRepository.GetUserById(userId);
                if (userCurrent == null)
                {
                    return new ApiResponse
                    {
                        statusCode = 404,
                        message = "Không tồn tại user có id này"
                    };
                }
                userCurrent.fullName = userUpdateDTO.fullName;
                userCurrent.image = userUpdateDTO.image;
                userCurrent.birthDay = userUpdateDTO.birthDay;
                userCurrent.gender = userUpdateDTO.gender;
                userCurrent.city = userUpdateDTO.city;
                userCurrent.district = userUpdateDTO.district;
                userUpdateDTO.ward = userUpdateDTO.ward;
                userCurrent.address = userUpdateDTO.address;
                userCurrent.phoneNumber = userUpdateDTO.phoneNumber;
                userCurrent.countViolation = userUpdateDTO.countViolation;

                await _userRepository.UpdateUser(userCurrent);
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    statusCode = 500,
                    message = ex.Message
                };
            }

        }
    }
}
