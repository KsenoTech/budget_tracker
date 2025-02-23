using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.ApplicationCore.Interfaces.Services;
using server.Infrastructure.BLL.Services;
using server.Infrastructure.DAL.Repositories;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Укажите порт вашего React-приложения
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


builder.Services.AddIdentity<Client, IdentityRole>(options => {
    options.User.RequireUniqueEmail = true;})
    .AddEntityFrameworkStores<AccountingForIncomeAndExpensesContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<AccountingForIncomeAndExpensesContext>();
builder.Services.AddControllers().
    AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler
    = ReferenceHandler.IgnoreCycles);

// Регистрация зависимостей
builder.Services.AddScoped<IDbRepository, DbRepository>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();



// Настройка JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("Starting the app");

app.Run();
