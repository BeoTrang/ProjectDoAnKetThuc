using CungCapAPI.Application.Interfaces;
using CungCapAPI.Application.Services;
using CungCapAPI.Models.DichVuTrong;
using CungCapAPI.Models.Redis;
using CungCapAPI.Models.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("JWT");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);


//var cookieSettings = builder.Configuration.GetSection("CookieSettings");
//string cookieDomain = cookieSettings.GetValue<string>("Domain");
//string cookiePath = cookieSettings.GetValue<string>("Path");

// Remove or replace this line, as it configures EF Core's internal Key type, which is not intended for DI or configuration.
// builder.Services.Configure<Key>(builder.Configuration.GetSection("Key"));

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
    //options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        if (context.Request.Cookies.ContainsKey("access_token"))
    //        {
    //            context.Token = context.Request.Cookies["access_token"];
    //        }
    //        return Task.CompletedTask;
    //    }
    //};
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetSection("Redis")["Configuration"];
    return ConnectionMultiplexer.Connect(config);
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<TaiKhoanRepository>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();

}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
