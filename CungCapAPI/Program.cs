using CungCapAPI.Application.Interfaces;
using CungCapAPI.Application.Services;
using CungCapAPI.Hubs;
using CungCapAPI.Models.DichVuTrong;
using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using CungCapAPI.MQTT;
using CungCapAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== JWT =====
var jwtSettings = builder.Configuration.GetSection("JWT");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

// ===== Authentication =====
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Cho phép token truyền qua query khi kết nối SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/deviceHub"))
                context.Token = accessToken;

            return Task.CompletedTask;
        }
    };
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7190")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ===== Services =====
builder.Services.AddSignalR();
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<TaiKhoanRepository>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetSection("Redis")["Configuration"];
    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddSingleton<InfluxService>();
builder.Services.AddSingleton<MqttService>();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

var mqttService = app.Services.GetRequiredService<MqttService>();
await mqttService.ConnectAsync();

app.UseHttpsRedirection();
app.UseRouting();

// ⚠️ Bắt buộc đặt ở đây
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<DeviceHub>("/deviceHub").RequireCors("AllowFrontend");
});

app.Run();
