using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace MedicalWeb.Tests;

public class SimpleIntegrationTests
{
    private readonly HttpClient _client;

    public SimpleIntegrationTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new System.Uri("http://localhost:5167");
    }

    [Fact]
    public async Task CreateDoctor_ReturnsCreated()
    {
        var newDoctor = new { name = "Тестовый Врач", specialty = "Тест" };
        var response = await _client.PostAsJsonAsync("/api/doctors", newDoctor);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetDoctors_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/doctors");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
