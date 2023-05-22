using AtonTestTask.Core;
using AtonTestTask.Services;
using AtonTestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace AtonTestTask.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Aton Test Task API",
                    Version = "v1",
                    Description = "Изначально существует пользователь с логином AdminTest и паролем 123123",
                    Contact = new OpenApiContact
                    {
                        Name = "GitHub",
                        Url = new Uri("https://github.com/Packword")
                    }
                });
                var basePath = AppContext.BaseDirectory;

                var xmlPath = Path.Combine(basePath, "AtonAPI.xml");
                options.IncludeXmlComments(xmlPath);
            });

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]));

            services.AddTransient<IUserService, UserService>();

            services.AddCors();
        }

    }
}