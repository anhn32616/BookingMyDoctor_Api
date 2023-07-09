using AutoMapper;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.Repositories;

using System.Security.Cryptography;
using System.Text;

namespace booking_my_doctor.Services
{

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IMapper mapper,
            IRoleRepository roleRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _emailService = emailService;
        }
        public async Task<string> Register(RegisterUserDto registerUserDto)
        {
            // Kiem tra email da ton tai hay chua
            bool check = await _userRepository.IsEmailAlreadyExists(registerUserDto.email);
            if (check)
            {
                throw new BadHttpRequestException("Email đã tồn tại!");
            }
            //Tao acc va return token
            using var hmac = new HMACSHA512();
            var passwordBytes = Encoding.UTF8.GetBytes(registerUserDto.password);
            var newUser = _mapper.Map<RegisterUserDto, User>(registerUserDto);

            newUser.image = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSetlcP-zX0tDv47hUaZ2Z3e_QKYclz3XzSOm9CJPlsqjYOWDDDP8dvk0wQZ3DQ6Ybyae4&usqp=CAU";

            var role = await _roleRepository.getRoleByName("ROLE_PATIENT");
            newUser.roleId = role.Id;
            newUser.gender = true;
            newUser.isDelete = false;
            newUser.PasswordSalt = hmac.Key;
            newUser.PasswordHash = hmac.ComputeHash(passwordBytes);
            newUser.isEmailVerified = false;
            await _userRepository.CreateUser(newUser);
            // Tao token xac thuc email
            var token = await _tokenService.CreateToken(newUser.email);
            newUser.token = token;
            // Gui email xac thuc tai khoan
            string body = $"<a href='http://localhost:3000/auth/verify-account?token={token}'>Click vào đây<a/> để xác thực tài khoản";
            _emailService.SendEmail(registerUserDto.email, "Xác thực tài khoản dịch vụ BOOKING MY DOCTOR", body);
            await _userRepository.UpdateUser(newUser);
            await _userRepository.IsSaveChanges();
            return token;
        }
        public async Task<ApiResponse> Login(UserLoginDto userLoginDto)
        {
            var user = await _userRepository.GetUserByEmail(userLoginDto.email);
            if (user == null)
            {
                return new ApiResponse
                {
                    statusCode = 400,
                    message = "email không tồn tại!"
                };
            }
            if (!user.isEmailVerified)
            {
                return new ApiResponse
                {
                    statusCode = 401,
                    message = "Email chưa được xác thực!"
                };
            }
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var passwordBytes = hmac.ComputeHash(
                Encoding.UTF8.GetBytes(userLoginDto.password)
            );

            for (int i = 0; i < user.PasswordHash.Length; i++)
            {
                if (user.PasswordHash[i] != passwordBytes[i])
                {
                    return new ApiResponse
                    {
                        statusCode = 401,
                        message = "Sai mật khẩu!"
                    };
                }
            }
            if (!user.isEmailVerified) return new ApiResponse
            {
                statusCode = 400,
                message = "Email chưa được xác thực"
            };
            if (user.isDelete == true) return new ApiResponse
            {
                statusCode = 400,
                message = "Tài khoản đã bị khóa",
            };
            var token = await _tokenService.CreateToken(user.email);
            return new ApiResponse
            {
                statusCode = 200,
                message = "Thành công",
                data = token
            };
        }

        public async Task<ApiResponse> VerifiedEmail(string token)
        {
            var result = await _userRepository.VerifiedEmail(token);
            await _userRepository.IsSaveChanges();
            if (result)
            {
                return new ApiResponse
                {
                    statusCode = 200,
                    message = "Thành công"
                };
            }
            return new ApiResponse
            {
                statusCode = 401,
                message = "Token không hợp lệ"
            };
        }

        public async Task<ApiResponse> SendMailVerified(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null) return new ApiResponse
            {
                statusCode = 404,
                message = "Không tồn tại user có email này"
            };
            _emailService.SendEmail(email, "Verified Email", user.token);
            return new ApiResponse
            {
                statusCode = 200,
                message = "Thành công"
            };
        }

        public async Task<ApiResponse> ForgotPassword(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null) return new ApiResponse()
            {
                statusCode = 400,
                message = "Email không tồn tại!"
            };

            if (!user.isEmailVerified) return new ApiResponse()
            {
                statusCode = 400,
                message = "Email của bạn chưa xác thực"
            };

            Random rnd = new Random();
            string newPass = "";
            for (int i = 0; i < 8; i++)
            {
                newPass += rnd.Next(0, 10).ToString();
            }

            using var hmac = new HMACSHA512();
            var passwordBytes = Encoding.UTF8.GetBytes(newPass);

            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(passwordBytes);

            await _userRepository.UpdateUser(user);
            await _userRepository.IsSaveChanges();


            _emailService.SendEmail(user.email, "[Booking My Doctor] Mật khẩu mới", "Mật khẩu mới của bạn là: " + newPass);
            return new ApiResponse()
            {
                statusCode = 200,
                message = "Thành công",
            };

        }
    }
}
