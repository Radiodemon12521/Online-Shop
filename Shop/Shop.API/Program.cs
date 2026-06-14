using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shop.API.Services;
using Shop.Database;
using System.Text;

namespace Shop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IJWTTokenService,JwtTokenService>();
            builder.Services.AddDbContext<AppDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetSection("ConnectionString").Value;
                options.UseNpgsql(connectionString);
            }, ServiceLifetime.Scoped);

            builder.Services.AddScoped<DapperContext>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWTTokenOptions:ValidIssuer"],
                        ValidAudience = builder.Configuration["JWTTokenOptions:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["JWTTokenOptions:IssuerSigningKey"]))
                    };
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5246",
                            "https://localhost:7077"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddAuthorization();

            var app = builder.Build(); // ← сначала Build()

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();

            app.UseCors("Frontend");        // ← 1. CORS
            app.UseAuthentication();        // ← 2. Аутентификация
            app.UseAuthorization();         // ← 3. Авторизация

            app.MapControllers();
            app.Run();
        }
    }
}