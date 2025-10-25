using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Data;
using ProjectManagerAPI.DTOs;
using ProjectManagerAPI.Models;
using BCrypt.Net;

namespace ProjectManagerAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<User?> GetUserByIdAsync(int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly TaskManagerContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(TaskManagerContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email || u.Username == registerDto.Username))
            {
                return null;
            }

            // Create new user
            var user = new User
            {
                Email = registerDto.Email,
                Username = registerDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Username);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Username = user.Username
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            // Generate token
            var token = _jwtService.GenerateToken(user.Id, user.Email, user.Username);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                Username = user.Username
            };
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}
