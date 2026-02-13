using EducationForEvery1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EducationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EducationDb")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.Run();

public partial class Program { }