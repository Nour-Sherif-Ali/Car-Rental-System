using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using Services.Abstractions;
using Shared.DTOS.User;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly UserDbContext _db;

        public UserService(UserDbContext db)
        {
            _db = db;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id, ClaimsPrincipal caller)
        {
            var callerId = int.Parse(caller.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = caller.IsInRole("Admin");

            if (!isAdmin && callerId != id)
                throw new UnauthorizedAccessException("You are not allowed to access this user");

            var user = await _db.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto dto, ClaimsPrincipal caller)
        {
            var callerId = int.Parse(caller.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = caller.IsInRole("Admin");

            if (!isAdmin && callerId != id)
                throw new UnauthorizedAccessException("You are not allowed to update this user");

            var user = await _db.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.FullName = dto.FullName ?? user.FullName;
            user.Email = dto.Email ?? user.Email;

            await _db.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task DeleteUserAsync(int id, ClaimsPrincipal caller)
        {
            if (!caller.IsInRole("Admin"))
                throw new UnauthorizedAccessException("Only admin can delete users");

            var user = await _db.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync(ClaimsPrincipal caller)
        {
            if (!caller.IsInRole("Admin"))
                throw new UnauthorizedAccessException("Only admin can see all users");

            return await _db.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync(); 
        }

    }
}

