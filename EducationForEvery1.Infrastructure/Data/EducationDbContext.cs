using EducationForEvery1.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EducationForEvery1.Infrastructure.Data;

// DbContext är EF Cores “huvudklass” för att prata med databasen.
// Den representerar en “session”/enhet av arbete mot databasen.
public class EducationDbContext : DbContext
{
    // Konstruktor som tar emot DbContextOptions via DI (Dependency Injection).
    // Options innehåller t.ex. connection string + vilken provider (SQL Server) som används.
    public EducationDbContext(DbContextOptions<EducationDbContext> options)
        : base(options) // Skickar options till DbContext-basklassen.
    {
    }

    // DbSet<T> motsvarar en tabell (eller vy) i databasen för typen T.
    // EF använder dessa för att kunna göra queries och skriva ändringar.
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseInstance> CourseInstances => Set<CourseInstance>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<CourseInstanceTeacher> CourseInstanceTeachers => Set<CourseInstanceTeacher>();

    // OnModelCreating körs när EF bygger upp modellen (kartläggningen mellan C#-klasser och DB).
    // Här konfigurerar du constraints, relationer, index osv (fluent API).
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfiguration för Course-tabellen
        modelBuilder.Entity<Course>(b =>
        {
            // Title ska vara obligatoriskt (NOT NULL) och max 200 tecken.
            b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        });

        // Konfiguration för CourseInstance-tabellen (kurstillfälle)
        modelBuilder.Entity<CourseInstance>(b =>
        {
            // Location är obligatoriskt och max 200 tecken.
            b.Property(x => x.Location).HasMaxLength(200).IsRequired();

            // DateOnly mappas här till SQL-typen "date" (inte datetime).
            // Det gör att databasen sparar datum utan tid.
            b.Property(x => x.StartDate).HasColumnType("date");
            b.Property(x => x.EndDate).HasColumnType("date");

            // Relation: CourseInstance har EN Course (HasOne),
            // och Course har ingen navigation här (WithMany() utan parameter).
            // CourseId är FK-kolumnen.
            b.HasOne(x => x.Course)
             .WithMany()
             .HasForeignKey(x => x.CourseId)
             // Om en Course tas bort => radera alla CourseInstances som pekar på den.
             // (Cascade delete i DB)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Konfiguration för Student-tabellen
        modelBuilder.Entity<Student>(b =>
        {
            // Förnamn/efternamn/email är obligatoriska och har maxlängder.
            b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Email).HasMaxLength(200).IsRequired();

            // Skapar ett unikt index på Email => inga två studenter kan ha samma email.
            // Detta blir oftast en UNIQUE constraint i DB.
            b.HasIndex(x => x.Email).IsUnique();
        });

        // Konfiguration för Teacher-tabellen
        modelBuilder.Entity<Teacher>(b =>
        {
            // Förnamn/efternamn/email är obligatoriska och har maxlängder.
            b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Email).HasMaxLength(200).IsRequired();

            // Unikt index på Email även här => inga två lärare kan dela email.
            b.HasIndex(x => x.Email).IsUnique();
        });

        // Konfiguration för Enrollment-tabellen (registrering)
        modelBuilder.Entity<Enrollment>(b =>
        {
            // EnrolledAtUtc (tidsstämpel) måste finnas (NOT NULL).
            b.Property(x => x.EnrolledAtUtc).IsRequired();

            // Status (t.ex. Active/Cancelled) måste finnas (NOT NULL).
            b.Property(x => x.Status).IsRequired();

            // Relation: Enrollment har EN Student.
            b.HasOne(x => x.Student)
             .WithMany() // Student har ingen navigation specificerad här.
             .HasForeignKey(x => x.StudentId)
             // Restrict betyder: om en Student har enrollments så kan den inte tas bort
             // utan att man först tar bort/ändrar dessa enrollments.
             .OnDelete(DeleteBehavior.Restrict);

            // Relation: Enrollment har EN CourseInstance.
            b.HasOne(x => x.CourseInstance)
             .WithMany()
             .HasForeignKey(x => x.CourseInstanceId)
             // Om CourseInstance tas bort => ta bort dess enrollments.
             .OnDelete(DeleteBehavior.Cascade);

            // Unikt index på kombinationen (StudentId, CourseInstanceId)
            // => en student kan bara registreras EN gång till samma kurstillfälle.
            b.HasIndex(x => new { x.StudentId, x.CourseInstanceId }).IsUnique();
        });

        // Konfiguration för join-tabellen CourseInstanceTeacher (many-to-many)
        modelBuilder.Entity<CourseInstanceTeacher>(b =>
        {
            // Sätter en sammansatt primärnyckel av två kolumner.
            // Det gör att samma Teacher inte kan kopplas till samma CourseInstance flera gånger.
            b.HasKey(x => new { x.CourseInstanceId, x.TeacherId });

            // Relation: join-raden pekar på en CourseInstance.
            b.HasOne(x => x.CourseInstance)
             .WithMany()
             .HasForeignKey(x => x.CourseInstanceId)
             // Om CourseInstance tas bort => ta bort rader i join-tabellen.
             .OnDelete(DeleteBehavior.Cascade);

            // Relation: join-raden pekar på en Teacher.
            b.HasOne(x => x.Teacher)
             .WithMany()
             .HasForeignKey(x => x.TeacherId)
             // Om Teacher tas bort => ta bort rader i join-tabellen.
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}