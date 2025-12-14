using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOS.User;

namespace Services.Abstractions
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(int id, ClaimsPrincipal caller);
        Task<UserResponseDto> UpdateUserAsync(int id, UpdateUserDto dto, ClaimsPrincipal caller);
        Task DeleteUserAsync(int id, ClaimsPrincipal caller);
        Task<List<UserResponseDto>> GetAllUsersAsync(ClaimsPrincipal caller);

    }
}
