using EducationForEvery1.Domain.Entities;

using EducationForEvery1.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

namespace EducationForEvery1.Endpoints;

// Static klass eftersom vi bara innehåller endpoint-mappningar
public static class CourseInstanceEndpoints
{
    // Extension method som gör att vi kan skriva:
    // app.MapCourseInstanceEndpoints();
    public static void MapCourseInstanceEndpoints(this IEndpointRouteBuilder app)
    {
        // ============================================
        // POST - Skapa nytt kurstillfälle
        // ============================================
        app.MapPost("/course-instances", async (EducationDbContext db, CourseInstanceCreateDto dto) =>
        {
            // Kontrollerar att den Course som dto.CourseId pekar på faktiskt finns i databasen
            var courseExists = await db.Courses.AnyAsync(c => c.Id == dto.CourseId);

            // Om kursen inte finns -> returnera 400 BadRequest
            if (!courseExists)
                return Results.BadRequest($"CourseId {dto.CourseId} finns inte.");

            // Försöker konvertera sträng till DateOnly
            // Om formatet är fel -> returnera 400
            if (!DateOnly.TryParse(dto.StartDate, out var start))
                return Results.BadRequest("StartDate måste vara i formatet yyyy-MM-dd (ex: 2026-03-01).");

            if (!DateOnly.TryParse(dto.EndDate, out var end))
                return Results.BadRequest("EndDate måste vara i formatet yyyy-MM-dd (ex: 2026-03-05).");

            // Skapar ett nytt CourseInstance-objekt med värdena
            var instance = new CourseInstance(
                dto.CourseId,
                start,
                end,
                dto.Location,
                dto.Capacity);

            // Lägger till objektet i DbContext (men sparar inte än)
            db.CourseInstances.Add(instance);

            // Sparar ändringen till databasen (INSERT körs här)
            await db.SaveChangesAsync();

            return Results.Created($"/course-instances/{instance.Id}", new
            {
                instance.Id,
                instance.CourseId,
                instance.StartDate,
                instance.EndDate,
                instance.Location,
                instance.Capacity
            });
        });

        // ============================================
        // GET - Hämta alla kurstillfällen
        // ============================================
        app.MapGet("/course-instances", async (EducationDbContext db) =>
        {
            // Hämtar alla kurstillfällen från databasen
            // Select används för att returnera ett anonymt objekt
            // (så vi inte skickar hela entityn direkt)
            var items = await db.CourseInstances
                .Select(ci => new
                {
                    ci.Id,
                    ci.CourseId,
                    ci.StartDate,
                    ci.EndDate,
                    ci.Location,
                    ci.Capacity
                })
                .ToListAsync();

            return Results.Ok(items);
        });

        // ============================================
        // GET - Hämta specifikt kurstillfälle
        // ============================================
        app.MapGet("/course-instances/{id:int}", async (EducationDbContext db, int id) =>
        {
            // Söker efter ett kurstillfälle med specifikt ID
            var item = await db.CourseInstances
                .Where(ci => ci.Id == id)
                .Select(ci => new
                {
                    ci.Id,
                    ci.CourseId,
                    ci.StartDate,
                    ci.EndDate,
                    ci.Location,
                    ci.Capacity
                })
                .FirstOrDefaultAsync();

            // Om inget hittas -> 404 NotFound
            return item is null
                ? Results.NotFound()
                : Results.Ok(item);
        });

        // ============================================
        // DELETE - Ta bort kurstillfälle
        // ============================================
        app.MapDelete("/course-instances/{id:int}", async (EducationDbContext db, int id) =>
        {
            // Försöker hitta objektet i databasen
            var item = await db.CourseInstances.FindAsync(id);

            // Om det inte finns -> 404
            if (item is null)
                return Results.NotFound();

            // Markerar objektet för borttagning
            db.CourseInstances.Remove(item);

            // Sparar ändringen (DELETE körs här)
            await db.SaveChangesAsync();

            // Returnerar 204 NoContent (standard för delete)
            return Results.NoContent();
        });
    }

    // ============================================
    // DTO (Data Transfer Object)
    // ============================================
    // Denna används när klienten skickar in data i POST
    // Vi använder string för datum för att slippa JSON-konverterare
    public record CourseInstanceCreateDto(
        int CourseId,
        string StartDate,
        string EndDate,     
        string Location,
        int Capacity);
}