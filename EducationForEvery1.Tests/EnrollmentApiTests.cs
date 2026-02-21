using System.Net.Http.Json;
using Xunit;

namespace EducationForEvery1.Tests;

public class EnrollmentApiTests : IClassFixture<TestApplication>
{
    private readonly HttpClient _client;

    public EnrollmentApiTests(TestApplication factory)
    {
        // Skapar en HttpClient kopplad till testservern (in-memory host)
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostEnrollment_Then_GetEnrollments_ContainsEnrollment()
    {
        // =========================
        // ARRANGE
        // =========================

        // 1) Skapa en Course
        var createCourse = new { title = "Kurs 1", description = "Beskrivning" };
        var courseResp = await _client.PostAsJsonAsync("/courses", createCourse);
        courseResp.EnsureSuccessStatusCode();

        var createdCourse = await courseResp.Content.ReadFromJsonAsync<CourseDto>();
        Assert.NotNull(createdCourse);

        // 2) Skapa en Student
        var createStudent = new { firstName = "Anna", lastName = "Test", email = "anna.test@example.com" };
        var studentResp = await _client.PostAsJsonAsync("/students", createStudent);
        studentResp.EnsureSuccessStatusCode();

        var createdStudent = await studentResp.Content.ReadFromJsonAsync<StudentDto>();
        Assert.NotNull(createdStudent);

        // 3) Skapa en CourseInstance (kopplad till kursen)
        var createInstance = new
        {
            courseId = createdCourse!.Id,
            startDate = "2026-03-01",
            endDate = "2026-03-05",
            location = "Online",
            capacity = 25
        };

        var instanceResp = await _client.PostAsJsonAsync("/course-instances", createInstance);
        instanceResp.EnsureSuccessStatusCode();

        var createdInstance = await instanceResp.Content.ReadFromJsonAsync<CourseInstanceDto>();
        Assert.NotNull(createdInstance);

        // =========================
        // ACT
        // =========================

        // 4) Skapa en Enrollment (student anmäler sig till kursinstansen)
        var createEnrollment = new
        {
            studentId = createdStudent!.Id,
            courseInstanceId = createdInstance!.Id,
            status = "Active"
        };

        var enrollResp = await _client.PostAsJsonAsync("/enrollments", createEnrollment);
        enrollResp.EnsureSuccessStatusCode();

        var createdEnrollment = await enrollResp.Content.ReadFromJsonAsync<EnrollmentDto>();
        Assert.NotNull(createdEnrollment);

        // 5) Hämta alla enrollments
        var enrollments = await _client.GetFromJsonAsync<List<EnrollmentDto>>("/enrollments");

        // =========================
        // ASSERT
        // =========================
        Assert.NotNull(enrollments);

        Assert.Contains(enrollments!, e =>
            e.StudentId == createdStudent.Id &&
            e.CourseInstanceId == createdInstance.Id);
    }

    // Små DTOs som matchar JSON-svaret från API:t
    public record CourseDto(int Id, string Title, string? Description);
    public record StudentDto(int Id, string FirstName, string LastName, string Email);

    public record CourseInstanceDto(
        int Id,
        int CourseId,
        string StartDate,
        string EndDate,
        string Location,
        int Capacity);

    public record EnrollmentDto(
        int Id,
        int StudentId,
        int CourseInstanceId,
        string EnrolledAtUtc,
        string Status);
}