using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Helpers;
using TranHuyenTran_2122110389.Services.Implementations;
using TranHuyenTran_2122110389.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Kết nối Database (Sử dụng tên server máy của bạn)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký các Service (Dependency Injection)
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IWorkScheduleService, WorkScheduleService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ISalaryService, SalaryService>();
builder.Services.AddScoped<IPositionService, PositionService>();

// 3. Cấu hình Authentication & JWT
var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtConfig["Key"] ?? "YourSuperSecretKey1234567890123456");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// 4. Cấu hình Swagger để hiện nút Authorize (Ổ khóa)
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
  {
      // 1. Bỏ qua vòng lặp tham chiếu
      options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

      // 2. Ép kiểu JSON thuần (Quan trọng: Ngăn chặn việc lặp lại metadata)
      options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;

      // 3. Bỏ qua các trường NULL
      options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;



  });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cafe Management API", Version = "v1" });

    // Định nghĩa bảo mật cho JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập Token theo cú pháp: Bearer [token_của_bạn]",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cafe API v1");
    c.RoutePrefix = "swagger"; // Đảm bảo đường dẫn là /swagger
});
// 5. Cấu hình Middleware
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cafe API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication(); // Giải mã token
app.UseAuthorization(); //ktra quyền

app.MapControllers();

app.Run();