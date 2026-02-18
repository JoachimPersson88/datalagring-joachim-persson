using EducationForEvery1.Domain.Entities;
using EducationForEvery1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducationForEvery1.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        // ============================================
        // POST - Skapa ny kurs
        // ============================================
        app.MapPost("/courses", async (EducationDbContext db, CourseCreateDto dto) =>
        {
            var course = new Course(dto.Title, dto.Description);
            db.Courses.Add(course);
            await db.SaveChangesAsync();

            return Results.Created($"/courses/{course.Id}", new
            {
                course.Id,
                course.Title,
                course.Description
            });
        });
        // ============================================
        // GET - Hämta alla kurser
        // ============================================
        app.MapGet("/courses", async (EducationDbContext db) =>
        {
            var courses = await db.Courses
                .Select(c => new { c.Id, c.Title, c.Description })
                .ToListAsync();

            return Results.Ok(courses);
        });
        // ============================================
        // GET - Hämta specifikt kurs
        // ============================================
        app.MapGet("/courses/{id:int}", async (EducationDbContext db, int id) =>
        {
            var course = await db.Courses
                .Where(c => c.Id == id)
                .Select(c => new { c.Id, c.Title, c.Description })
                .FirstOrDefaultAsync();

            return course is null ? Results.NotFound() : Results.Ok(course);
        });
        // ============================================
        // DELETE - Ta bort kurs
        // ============================================
        app.MapDelete("/courses/{id:int}", async (EducationDbContext db, int id) =>
        {
            var course = await db.Courses.FindAsync(id);
            if (course is null) return Results.NotFound();

            db.Courses.Remove(course);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    // DTO ligger här för enkelhet. (Sen kan vi flytta DTOs till Application.)
    public record CourseCreateDto(string Title, string? Description);
}