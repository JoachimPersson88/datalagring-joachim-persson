using EducationForEvery1.Domain.Entities;
using EducationForEvery1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducationForEvery1.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        // ============================================
        // POST - Skapa nytt studentID
        // ============================================
        app.MapPost("/students", async (EducationDbContext db, StudentCreateDto dto) =>
        {
            var student = new Student(dto.FirstName, dto.LastName, dto.Email);
            db.Students.Add(student);
            await db.SaveChangesAsync();

            return Results.Created($"/students/{student.Id}", new
            {
                student.Id,
                student.FirstName,
                student.LastName,
                student.Email
            });
        });
        // ============================================
        // GET - Hämta alla studentID
        // ============================================
        app.MapGet("/students", async (EducationDbContext db) =>
        {
            var students = await db.Students
                .Select(s => new { s.Id, s.FirstName, s.LastName, s.Email })
                .ToListAsync();

            return Results.Ok(students);
        });
        // ============================================
        // GET - Hämta specifik studentID
        // ============================================
        app.MapGet("/students/{id:int}", async (EducationDbContext db, int id) =>
        {
            var student = await db.Students
                .Where(s => s.Id == id)
                .Select(s => new { s.Id, s.FirstName, s.LastName, s.Email })
                .FirstOrDefaultAsync();

            return student is null ? Results.NotFound() : Results.Ok(student);
        });
        // ============================================
        // DELETE - Ta bort specifik studentID
        // ============================================
        app.MapDelete("/students/{id:int}", async (EducationDbContext db, int id) =>
        {
            var student = await db.Students.FindAsync(id);
            if (student is null) return Results.NotFound();

            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    public record StudentCreateDto(string FirstName, string LastName, string Email);
}