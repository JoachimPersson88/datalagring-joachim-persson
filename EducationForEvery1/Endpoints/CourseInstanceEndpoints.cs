using EducationForEvery1.Domain.Entities;
using EducationForEvery1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducationForEvery1.Endpoints;

public static class CourseInstanceEndpoints
{
    public static void MapCourseInstanceEndpoints(this IEndpointRouteBuilder app)
    {
        // Skapa nytt kurstillfälle
        app.MapPost("/course-instances", async (EducationDbContext db, CourseInstanceCreateDto dto) =>
        {
            // Kontroll att Course finns
            var courseExists = await db.Courses.AnyAsync(c => c.Id == dto.CourseId);
            if (!courseExists) return Results.BadRequest($"CourseId {dto.CourseId} finns inte.");

            var instance = new CourseInstance(dto.CourseId, dto.StartDate, dto.EndDate, dto.Location);

            db.CourseInstances.Add(instance);
            await db.SaveChangesAsync();

            return Results.Created($"/course-instances/{instance.Id}", new
            {
                instance.Id,
                instance.CourseId,
                instance.StartDate,
                instance.EndDate,
                instance.Location
            });
        });

        // Lista alla kurstillfällen
        app.MapGet("/course-instances", async (EducationDbContext db) =>
        {
            var items = await db.CourseInstances
                .Select(ci => new { ci.Id, ci.CourseId, ci.StartDate, ci.EndDate, ci.Location })
                .ToListAsync();

            return Results.Ok(items);
        });

        // Hämta ett kurstillfälle
        app.MapGet("/course-instances/{id:int}", async (EducationDbContext db, int id) =>
        {
            var item = await db.CourseInstances
                .Where(ci => ci.Id == id)
                .Select(ci => new { ci.Id, ci.CourseId, ci.StartDate, ci.EndDate, ci.Location })
                .FirstOrDefaultAsync();

            return item is null ? Results.NotFound() : Results.Ok(item);
        });

        // Ta bort ett kurstillfälle
        app.MapDelete("/course-instances/{id:int}", async (EducationDbContext db, int id) =>
        {
            var item = await db.CourseInstances.FindAsync(id);
            if (item is null) return Results.NotFound();

            db.CourseInstances.Remove(item);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    public record CourseInstanceCreateDto(int CourseId, DateTime StartDate, DateTime EndDate, string Location);
}