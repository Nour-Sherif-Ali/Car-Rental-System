using System.Text;
using Car_Rental_System.CustomMiddleWare;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistance.Data;
using Persistance.Data.DataSeed;
using Presentation.JWT;
using Services.Abstractions;
using Services;
using Services.CarService;
using Services.BookingService;
using Domain.Contracts;
using Persistance.Repositories;

namespace Car_Rental_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContexts
            builder.Services.AddDbContext<CarRentalDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICarService, CarService>();
            builder.Services.AddScoped<IBookingService, BookingService>();

            // Add these registrations
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICarService, CarService>();

            builder.Services.AddScoped<IAuthService, AuthService>();
 
            // JWT Setup
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWTOptions"));
            builder.Services.AddScoped<JwtTokenGenerator>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", options =>
            {
                var jwtOptions = builder.Configuration
                    .GetSection("JWTOptions")
                    .Get<JwtOptions>();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Data Seeding
            using (var scope = app.Services.CreateScope())
            {
                // Cars seeding
                var carDb = scope.ServiceProvider.GetRequiredService<CarRentalDbContext>();
                var carSeeder = new DataSeeding(carDb);
                await carSeeder.SeedAsync();

                // Users seeding
                var userDb = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                var userSeeder = new UserDataSeeding(userDb);
                await userSeeder.SeedAsync();
            }



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
