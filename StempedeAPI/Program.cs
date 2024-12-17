using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using DataAccess;
using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories.Implementations;
using DataAccess.Data;
using BusinessLogic.Auth.Services.Implementation;
using BusinessLogic.Auth.Services.Interfaces;
using BusinessLogic.Services.Implementation;
using BusinessLogic.Services.Interfaces;
using BusinessLogic.Configurations.MappingProfiles;
using DataAccess.Entities;

namespace StempedeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            var app = builder.Build();
            ConfigureMiddleware(app);
            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    policy => policy.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
            });

            // Database configuration
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DB"))
            );

            // Th�m Authentication v� Authorization services
            builder.Services.AddAuthentication();
            builder.Services.AddAuthorization();

            // Register Repositories and Services
            RegisterRepositories(builder.Services);
            RegisterServices(builder.Services);

            // AutoMapper configuration
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

            // Add Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ?�ng th? t? middleware
            app.UseCors("AllowReactApp");

            app.UseRouting();

            app.UseAuthentication(); // Ph?i ??t tr??c UseAuthorization
            app.UseAuthorization();  // Ph?i ??t sau UseRouting

            app.MapControllers();    // Thay th? UseEndpoints
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ILabService, LabService>();
            services.AddScoped<ISubcategoryService, SubcategoryService>();
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}