
using System.Net.Http.Json;


using Xunit;

namespace EducationForEvery1.Tests;

// CourseApiTests är en testklass som innehåller integrationstester för kurs-API:t.
public class CourseApiTests : IClassFixture<TestApplication>
{
    // HttpClient används för att göra HTTP-anrop mot vår testserver.
    private readonly HttpClient _client;
    // Konstruktor som tar emot en instans av TestApplication via IClassFixture.
    public CourseApiTests(TestApplication factory)
    {
        // Vi skapar en HttpClient som kommer att användas för att göra anrop mot vår testserver.
        _client = factory.CreateClient();
    }

    // =========================
    // ARRANGE
    // =========================

    [Fact]
    // Testmetod som testar att vi kan skapa en kurs och sedan hämta den via GET /courses.
    public async Task PostCourse_Then_GetCourses_ReturnsCreatedCourse()
    {
        // Vi skapar ett anonymt objekt som representerar den kurs vi vill skapa.
        var create = new
        {
            title = "Testkurs",
            description = "Beskrivning"
        };

        // =========================
        // ACT
        // =========================

        // Vi skickar en POST-förfrågan till /courses med det skapade objektet som JSON.
        var postResponse = await _client.PostAsJsonAsync("/courses", create);
        // Vi kollar om POST-förfrågan lyckades. Om inte, läser vi svaret som text och kastar ett undantag med statuskod och svarstext.
        if (!postResponse.IsSuccessStatusCode)
        {
            // Om POST-förfrågan misslyckades, läs svaret som text och kasta ett undantag med statuskod och svarstext.
            var body = await postResponse.Content.ReadAsStringAsync();
            // Vi kastar ett undantag som innehåller statuskoden och svaret från servern för att underlätta felsökning.
            throw new Exception($"POST /courses failed: {(int)postResponse.StatusCode}\n{body}");
        }

        // =========================
        // ASSERT
        // =========================

        // Om POST-förfrågan lyckades, fortsätt som vanligt.
        postResponse.EnsureSuccessStatusCode();
        // Vi skickar en GET-förfrågan till /courses för att hämta alla kurser.
        var courses = await _client.GetFromJsonAsync<List<CourseDto>>("/courses");
        // Vi kollar att listan med kurser inte är null.
        Assert.NotNull(courses);
        // Vi kollar att det finns en kurs i listan som har titeln "Testkurs".
        Assert.Contains(courses!, c => c.Title == "Testkurs");
    }
    // CourseDto är en enkel record-typ som används för att deserialisera kursdata från API:t.
    public record CourseDto(int Id, string Title, string? Description);
}