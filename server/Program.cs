using Microsoft.AspNetCore.Identity;
using server.ApplicationCore.DomModels;
using server.ApplicationCore.Interfaces.Repositories;
using server.Infrastructure.DAL.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
    builder =>
    {
        builder.WithOrigins("http://localhost:3000")
       .AllowAnyHeader()
       .AllowAnyMethod();
    });
});

// Add services to the container.

builder.Services.AddIdentity<User, IdentityRole>(options => {
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AccountingForIncomeAndExpensesContext>().AddDefaultTokenProviders();
builder.Services.AddDbContext<AccountingForIncomeAndExpensesContext>();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler
                                                                        = ReferenceHandler.IgnoreCycles);

builder.Services.AddScoped<IDbRepository, DbRepository>();


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

app.UseCors();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("Starting the app");

app.Run();
