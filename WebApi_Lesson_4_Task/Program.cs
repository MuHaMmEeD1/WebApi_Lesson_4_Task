using Microsoft.EntityFrameworkCore;
using WebApi_Lesson_4_Task.Data;
using WebApi_Lesson_4_Task.Repositories.Abstract;
using WebApi_Lesson_4_Task.Repositories.Concreat;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IMovieRepository, MovieReopsitory>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddHostedService<MyBackgroundService>();

var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<MovieDbContext>(option => { option.UseSqlServer(connectionString); });

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    //app.UseSwagger();
    //app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
