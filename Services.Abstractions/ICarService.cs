using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.DTOS.Car;
using Shared.DTOS.Pagination;

namespace Services.Abstractions
{
    public interface ICarService
    {
        Task<PagedResult<CarResponseDto>> GetAllCarsAsync(CarQueryParameters parameters);
        Task<CarResponseDto> GetCarByIdAsync(int id);
        Task<CarResponseDto> CreateCarAsync(CreateCarDto dto);
        Task<CarResponseDto> UpdateCarAsync(int id, UpdateCarDto dto);
        Task DeleteCarAsync(int id);
        Task UpdateCarImageAsync(int id, IFormFile image);
        Task<string> SaveCarImage(IFormFile image);
    }
}
