using appointment_my_doctor.Services.Appointment;
using booking_my_clinic.Services;
using booking_my_doctor.Data;
using booking_my_doctor.Profiles;
using booking_my_doctor.Repositories;
using booking_my_doctor.Repositories.Appoiment;
using booking_my_doctor.Services;
using DatingApp.API.Data.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using schedule_my_doctor.Services.Schedule;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7042, listenOptions =>
    {
        listenOptions.UseHttps();
    });
});
// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;
services.AddCors(o =>
    o.AddPolicy("CorsPolicy", builder =>
        builder.WithOrigins("http://localhost:3000", "http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod()));

var connectionString = builder
    .Configuration.GetConnectionString("MyDB");
services.AddDbContext<MyDbContext>
(option =>
{
    option.UseSqlServer(connectionString);
}, ServiceLifetime.Transient);
services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

//repository
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IRoleRepository, RoleRepository>();
services.AddScoped<IDoctorRepository, DoctorRepository>();
services.AddScoped<IClinicRepository, ClinicRepository>();
services.AddScoped<IHospitalRepository, HospitalRepository>();
services.AddScoped<ISpeciatlyRepository, SpeciatlyRepository>();
services.AddScoped<IScheduleRepository, ScheduleRepository>();
services.AddScoped<IAppointmentRepository, AppointmentRepository>();

//services
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IDoctorService, DoctorService>();
services.AddScoped<IHospitalService, HospitalService>();
services.AddScoped<IClinicService, ClinicService>();
services.AddScoped<ISpeciatlyService, SpeciatlyService>();
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<IScheduleService, ScheduleService>();
services.AddScoped<IAppointmentService, AppointmentService>();

// Background service
services.AddHostedService<AppointmentBackgroundService>();
services.AddHostedService<ScheduleBackgroundService>();

//mapper
services.AddAutoMapper(typeof(MapperProfile).Assembly);
//JWT
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]))
        };
    });
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Swagger Booking My Doctor",
        Version = "v1",
        Description = "An ASP.NET Core Web API for project Booking-My-Doctor By Nguyen Trong Anh",
        Contact = new OpenApiContact
        {
            Name = "Nguyen Trong Anh",
            Url = new Uri("https://www.facebook.com/anhnguyen53")
        },
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Authorization header using the Bearer scheme. 
                Enter 'Bearer' [space] and then your token in the text input below.
                Example: 'Bearer 12345abcdef",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
        {
            new OpenApiSecurityScheme
            {
            Reference = new OpenApiReference
                {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
                },
                //Scheme = "Bearer",
                //Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
            }
        });
});
var app = builder.Build();
using var scope = app.Services.CreateScope();
var servicesProvider = scope.ServiceProvider;
try
{
    var context = servicesProvider.GetRequiredService<MyDbContext>();
    context.Database.Migrate();
    Seed.SeedUsers(context);
}
catch (Exception e)
{
    var logger = servicesProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "Migration failed");
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
