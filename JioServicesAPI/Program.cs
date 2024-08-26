using System;
using JioServicesAPI.Data;
using JioServicesAPI.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorUI",
        policy =>
        {
            policy.WithOrigins("https://localhost:7176") // Replace with your Blazor UI URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register custom services
builder.Services.AddSingleton<NotificationService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowBlazorUI");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
