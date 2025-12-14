using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Data.DataSeed
{
    public class DataSeeding
    {
        private readonly CarRentalDbContext _carDb;

        public DataSeeding(CarRentalDbContext carDb)
        {
            _carDb = carDb;
        }

        public async Task SeedAsync()
        {
            // Apply pending migrations
            var pendingMigrations = await _carDb.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
                await _carDb.Database.MigrateAsync();

            // Seed Cars from JSON
            if (!_carDb.Cars.Any())
            {
                var carsData = File.OpenRead(@"..\Persistance\Data\DataSeed\cars.json");

                // Add JsonSerializerOptions to handle property names correctly
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                var cars = await JsonSerializer.DeserializeAsync<List<Car>>(carsData, options);

                if (cars?.Any() == true)
                {
                    await _carDb.Cars.AddRangeAsync(cars);
                    await _carDb.SaveChangesAsync(); // Save here to commit the data
                }
            }
        }
    }
}


