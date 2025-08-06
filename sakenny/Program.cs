using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sakenny.API.Mapping;
using sakenny.Application.Interfaces;
using sakenny.Application.Services;
using sakenny.DAL;
using sakenny.DAL.Interfaces;
using sakenny.DAL.Models;
using sakenny.DAL.Repository;
using sakenny.ServiceExtensions;
using sakenny.Services;
using sakenny.Models;
using Stripe;
using sakenny.Application.DTO;
using Google.Apis.Auth;
using System.Security.Cryptography;

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
            builder.Services.AddScoped<IPropertyTypeService, PropertyTypeService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IPropertyService, PropertyService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped(typeof(IDeleteUpdate<>), typeof(DeleteUpdateRepository<>));
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<UserUpdateDeleteProfileService>();
            builder.Services.AddScoped<LoginService>();
            builder.Services.AddScoped<ICheckoutService, sakenny.Application.Services.CheckoutService>();
            builder.Services.AddScoped<IReviewService, sakenny.Application.Services.ReviewService>();
            builder.Services.AddScoped<IRentedPropertyService, RentedPropertyService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<GoogleAuthService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
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

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return new BadRequestObjectResult(errors);
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerAuth();

            builder.Services.AddScoped<BlobService>();
            builder.Services.AddScoped<IImageService, ImageService>();

            builder.Services.AddScoped<AdminService>();
            //Stripe configuration
            //Retrieve the Stripe API keys from appsettings.json
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseCors("AllowAngularApp");

            app.UseAuthorization();

            //app.UseCors("AllowAll");

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