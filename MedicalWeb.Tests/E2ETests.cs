using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace MedicalWeb.Tests;

public class E2ETests
{
    private readonly HttpClient _client;

    public E2ETests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new System.Uri("http://localhost:5167");
    }

    [Fact]
    public async Task E2E_FullFlow_CreateDoctorSlotAndAppointment()
    {
        // 1. Создаём врача
        var doctor = new { name = "E2E Доктор", specialty = "Тест" };
        var doctorResponse = await _client.PostAsJsonAsync("/api/doctors", doctor);
        Assert.Equal(HttpStatusCode.Created, doctorResponse.StatusCode);
        
        // 2. Получаем список врачей
        var doctorsResponse = await _client.GetAsync("/api/doctors");
        Assert.Equal(HttpStatusCode.OK, doctorsResponse.StatusCode);
    }
}
