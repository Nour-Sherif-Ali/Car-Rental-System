using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Services.Abstractions;
using Presentation.JWT;
using Persistance.Data;
using Shared.DTOS.User;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserDbContext _db;
        private readonly JwtTokenGenerator _jwt;

        public AuthService(UserDbContext db, JwtTokenGenerator jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email already exists");

            var newUser = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            
            return "User registered successfully";
        }


        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            return _jwt.GenerateToken(user);
        }
    }
}
