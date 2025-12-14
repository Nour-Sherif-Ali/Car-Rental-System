using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using Services.Abstractions;
using Services.Specifications;
using Shared.DTOS.Car;
using Shared.DTOS.Pagination;

namespace Services.CarService
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public CarService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        // GET ALL WITH PAGINATION, FILTERING, AND SORTING
        public async Task<PagedResult<CarResponseDto>> GetAllCarsAsync(CarQueryParameters parameters)
        {
            // Create specifications
            var spec = new CarWithFiltersSpecification(parameters);
            var countSpec = new CarWithFiltersForCountSpecification(parameters);

            // Get data using repository
            var cars = await _unitOfWork.Repository<Car>().GetAsync(spec);
            var totalCount = await _unitOfWork.Repository<Car>().CountAsync(countSpec);

            // Map to DTOs
            var carDtos = cars.Select(c => MapToDto(c)).ToList();

            return new PagedResult<CarResponseDto>
            {
                Items = carDtos,
                TotalCount = totalCount,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }

        // GET CAR BY ID
        public async Task<CarResponseDto> GetCarByIdAsync(int id)
        {
            var spec = new CarByIdSpecification(id);
            var cars = await _unitOfWork.Repository<Car>().GetAsync(spec);
            var car = cars.FirstOrDefault();

            if (car == null)
                throw new KeyNotFoundException("Car not found");

            return MapToDto(car);
        }

        // CREATE CAR
        public async Task<CarResponseDto> CreateCarAsync(CreateCarDto dto)
        {
            var car = new Car
            {
                Name = dto.Name,
                Brand = dto.Brand,
                PricePerDay = dto.PricePerDay,
                Available = true,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Car>().AddAsync(car);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(car);
        }

        // UPDATE CAR
        public async Task<CarResponseDto> UpdateCarAsync(int id, UpdateCarDto dto)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);

            if (car == null || car.IsDeleted)
                throw new KeyNotFoundException("Car not found");

            // Update fields
            car.Name = dto.Name ?? car.Name;
            car.Brand = dto.Brand ?? car.Brand;
            car.PricePerDay = dto.PricePerDay ?? car.PricePerDay;

            if (dto.Available.HasValue)
                car.Available = dto.Available.Value;

            // Update image URL if provided
            if (!string.IsNullOrEmpty(dto.Imageurl))
            {
                car.ImageUrl = dto.Imageurl;
            }

            car.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Car>().Update(car);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(car);
        }

        // DELETE CAR (SOFT DELETE)
        public async Task DeleteCarAsync(int id)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);

            if (car == null || car.IsDeleted)
                throw new KeyNotFoundException("Car not found");

            car.IsDeleted = true;
            car.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Car>().Update(car);
            await _unitOfWork.SaveChangesAsync();
        }

        // UPLOAD/UPDATE CAR IMAGE
        public async Task UpdateCarImageAsync(int id, IFormFile image)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);

            if (car == null || car.IsDeleted)
                throw new KeyNotFoundException("Car not found");

            // Delete old image if exists
            if (!string.IsNullOrEmpty(car.ImageUrl))
            {
                var oldPath = Path.Combine(_env.WebRootPath, car.ImageUrl.TrimStart('/'));
                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            // Save new image
            car.ImageUrl = await SaveCarImage(image);
            car.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Car>().Update(car);
            await _unitOfWork.SaveChangesAsync();
        }

        // SAVE IMAGE TO DISK
        public async Task<string> SaveCarImage(IFormFile image)
        {
            if (image == null)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images/cars");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/images/cars/{fileName}";
        }

        // PRIVATE MAPPER
        private CarResponseDto MapToDto(Car car)
        {
            return new CarResponseDto
            {
                Id = car.Id,
                Name = car.Name,
                Brand = car.Brand,
                PricePerDay = car.PricePerDay,
                Available = car.Available,
                ImageUrl = car.ImageUrl
            };
        }
    }

}
