using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TransportSystem.Infrastructure.Identity;
using TransportSystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TransportSystem.Infrastructure")));

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Set to true in production
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSecret = builder.Configuration["JWT:Secret"]
    ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["JWT:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer not configured");
var jwtAudience = builder.Configuration["JWT:Audience"]
    ?? throw new InvalidOperationException("JWT Audience not configured");

var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register JWT Token Service
builder.Services.AddScoped<JwtTokenService>();

// Register Database Initializer
builder.Services.AddScoped<DatabaseInitializer>();

// Register DbContext interface
builder.Services.AddScoped<TransportSystem.Application.Persistence.IApplicationDbContext>(provider =>
    provider.GetRequiredService<ApplicationDbContext>());

// Register Station Use Cases
builder.Services.AddScoped<TransportSystem.Application.Stations.UseCases.CreateStationUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Stations.UseCases.UpdateStationUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Stations.UseCases.DeleteStationUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Stations.UseCases.GetStationUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Stations.UseCases.GetAllStationsUseCase>();

// Register Driver Use Cases
builder.Services.AddScoped<TransportSystem.Application.Drivers.UseCases.CreateDriverUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Drivers.UseCases.UpdateDriverUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Drivers.UseCases.DeleteDriverUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Drivers.UseCases.GetDriverUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Drivers.UseCases.GetAllDriversUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Drivers.UseCases.ChangeDriverStatusUseCase>();

// Register Vehicle Use Cases
builder.Services.AddScoped<TransportSystem.Application.Vehicles.UseCases.CreateVehicleUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Vehicles.UseCases.UpdateVehicleUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Vehicles.UseCases.DeleteVehicleUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Vehicles.UseCases.GetVehicleUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Vehicles.UseCases.GetAllVehiclesUseCase>();
builder.Services.AddScoped<TransportSystem.Application.Vehicles.UseCases.ChangeVehicleStatusUseCase>();

// Configure Swagger/OpenAPI with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Transport System API",
        Version = "v1",
        Description = "City Transport Information System API"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Initialize and seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Transport System API v1");
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
