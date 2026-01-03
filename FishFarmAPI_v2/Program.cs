using System;
using FishFarm.DataAccessLayer;
using FishFarm.Repositories;
using FishFarm.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200",
            "https://eau-claire.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                ))
        };
    });

builder.Services.AddAuthorization();


//dang ky service
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<DeviceDAO>();
builder.Services.AddScoped<UserProfileDAO>();
builder.Services.AddScoped<RefreshTokenDAO>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUtils, Utils>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<OtpService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var provider = builder.Configuration["DB_PROVIDER"];
var conn = builder.Configuration.GetConnectionString("MyDbConnection");

//For supabase testing environment only
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("System.Net.DontEnableSystemDefaultDns", true);
AppContext.SetSwitch("System.Net.DisableIPv6", true);

builder.Services.AddDbContext<FishFarmContext>(options =>
{
    if (provider == "Postgres")
        options.UseNpgsql(conn)
        .UseSnakeCaseNamingConvention();  // ép EF dùng kiểu snake_case tương thích với PostgreSQL - Supabase

    else
        options.UseSqlServer(conn);
});


builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("otp", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    
    options.RejectionStatusCode = 429;
});

//builder.Configuration.AddEnvironmentVariables();

builder.Services.AddMemoryCache();

Console.WriteLine("Connection: " + builder.Configuration.GetConnectionString("MyDbConnection"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization(); 

app.UseCors();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
