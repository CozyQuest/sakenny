using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sakenny.API.Mapping;
using sakenny.Application.Interfaces;
using sakenny.Application.Services;
using sakenny.DAL;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Repository;
using sakenny.ServiceExtensions;
using sakenny.Services;

namespace sakenny
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            builder.Services.AddScoped<IPropertyServicesService, PropertyServicesService>();
            builder.Services.AddScoped<IPropertyService, PropertyService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped(typeof(IDeleteUpdate<>), typeof(DeleteUpdateRepository<>));
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<LoginService>();
            builder.Services.AddScoped<ICheckoutService, CheckoutService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            // Configure RefreshTokenProviderOptions
            builder.Services.Configure<RefreshTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(builder.Configuration.GetValue<int>("Jwt:RefreshTokenExpiryDays"));
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDBContext>()
              .AddDefaultTokenProviders()
              .AddTokenProvider<RefreshTokenProvider<IdentityUser>>("RefreshTokenProvider");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserRole", policy => policy.RequireRole("User"));
                options.AddPolicy("HostRole", policy => policy.RequireRole("Host"));
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerAuth();

            builder.Services.AddScoped<BlobService>();
            builder.Services.AddScoped<IImageService, ImageService>();

            builder.Services.AddScoped<AdminService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.MapControllers();
            // Create roles at startup
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await AssignRoles(roleManager);
            }
            app.Run();
        }

        private static async Task<bool> AssignRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Host"))
                await roleManager.CreateAsync(new IdentityRole("Host"));

            return true;
        }
    }
}