using System.Text.Json.Serialization;
using api.context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddCors();


string mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"mySqlConnectionStr: {mySqlConnectionStr}");

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql (mySqlConnectionStr, ServerVersion. AutoDetect (mySqlConnectionStr)));

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseCors(options => options.WithOrigins("http://localhost:5155").AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
