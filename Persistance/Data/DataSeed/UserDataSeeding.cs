using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Persistance.Data.DataSeed
{
    public class UserDataSeeding
    {
        private readonly UserDbContext _context;

        public UserDataSeeding(UserDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (!_context.Users.Any())
            {
                var admin = new User
                {
                    FullName = "Admin User",
                    Email = "admin@admin.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(admin);
                await _context.SaveChangesAsync();
            }
        }
    }

}
