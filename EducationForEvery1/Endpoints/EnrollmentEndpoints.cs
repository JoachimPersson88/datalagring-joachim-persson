using EducationForEvery1.Domain.Entities;
using EducationForEvery1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EducationForEvery1.Endpoints;

public static class EnrollmentEndpoints
{
    public static void MapEnrollmentEndpoints(this IEndpointRouteBuilder app)
    {
        // ============================================
        // POST - Registrera en student till ett kurstillfälle
        // ============================================
        app.MapPost("/enrollments", async (EducationDbContext db, EnrollmentCreateDto dto) =>
        {
            // Kontroll: Student finns?
            var studentExists = await db.Students.AnyAsync(s => s.Id == dto.StudentId);
            if (!studentExists) return Results.BadRequest($"StudentId {dto.StudentId} finns inte.");

            // Kontroll: CourseInstance finns?
            var instance = await db.CourseInstances
                .Where(ci => ci.Id == dto.CourseInstanceId)
                .Select(ci => new { ci.Id, ci.Capacity })
                .FirstOrDefaultAsync();

            if (instance is null) return Results.BadRequest($"CourseInstanceId {dto.CourseInstanceId} finns inte.");

            // Kontroll: redan registrerad?
            var alreadyEnrolled = await db.Enrollments.AnyAsync(e =>
                e.StudentId == dto.StudentId &&
                e.CourseInstanceId == dto.CourseInstanceId &&
                e.Status == EnrollmentStatus.Active);

            if (alreadyEnrolled) return Results.Conflict("Studenten är redan registrerad på detta kurstillfälle.");

            // Kontroll: plats kvar?
            var activeCount = await db.Enrollments.CountAsync(e =>
                e.CourseInstanceId == dto.CourseInstanceId &&
                e.Status == EnrollmentStatus.Active);

            if (activeCount >= instance.Capacity)
                return Results.Conflict("Kurstillfället är fullt.");

            // Skapa registreringen
            var enrollment = new Enrollment(dto.StudentId, dto.CourseInstanceId);

            db.Enrollments.Add(enrollment);
            await db.SaveChangesAsync();

            return Results.Created($"/enrollments/{enrollment.Id}", new
            {
                enrollment.Id,
                enrollment.StudentId,
                enrollment.CourseInstanceId,
                enrollment.EnrolledAtUtc,
                enrollment.Status
            });
        });

        // ============================================
        // GET - Lista registreringar
        // ============================================
        app.MapGet("/enrollments", async (EducationDbContext db) =>
        {
            var enrollments = await db.Enrollments
                .Select(e => new
                {
                    e.Id,
                    e.StudentId,
                    e.CourseInstanceId,
                    e.EnrolledAtUtc,
                    e.Status
                })
                .ToListAsync();

            return Results.Ok(enrollments);
        });

        // ============================================
        // GET - Hämta en specifik enrollment
        // ============================================
        app.MapGet("/enrollments/{id:int}", async (EducationDbContext db, int id) =>
        {
            var item = await db.Enrollments
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    e.Id,
                    e.StudentId,
                    e.CourseInstanceId,
                    e.EnrolledAtUtc,
                    e.Status
                })
                .FirstOrDefaultAsync();

            return item is null ? Results.NotFound() : Results.Ok(item);
        });

        // ============================================
        // POST - Avregistrera (Cancel) en enrollment
        // ============================================
        app.MapPost("/enrollments/{id:int}/cancel", async (EducationDbContext db, int id) =>
        {
            var enrollment = await db.Enrollments.FindAsync(id);
            if (enrollment is null) return Results.NotFound();

            enrollment.Cancel();
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }

    public record EnrollmentCreateDto(int StudentId, int CourseInstanceId);
}