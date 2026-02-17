using EducationForEvery1.Endpoints;
using EducationForEvery1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EducationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EducationDb")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapCourseEndpoints();
app.MapStudentEndpoints();

app.Run();

public partial class Program { }