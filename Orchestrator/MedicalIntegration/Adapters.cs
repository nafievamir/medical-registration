using System.Text;
using Newtonsoft.Json;
using Serilog;

namespace MedicalIntegration;

public interface IDoctorsAdapter
{
    Task<List<Doctor>> GetDoctorsAsync();
}

public class DoctorsAdapter : IDoctorsAdapter
{
    private readonly HttpClient _httpClient;
    public DoctorsAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Doctor>> GetDoctorsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:5001/doctors");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var doctors = JsonConvert.DeserializeObject<List<Doctor>>(json);
            Log.Information("Получено {Count} врачей", doctors?.Count ?? 0);
            return doctors ?? new List<Doctor>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при получении списка врачей");
            throw;
        }
    }
}

public interface IScheduleAdapter
{
    Task<SlotResponse> AddSlotAsync(SlotRequest slotRequest);
}

public class ScheduleAdapter : IScheduleAdapter
{
    private readonly HttpClient _httpClient;
    public ScheduleAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SlotResponse> AddSlotAsync(SlotRequest slotRequest)
    {
        try
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(slotRequest),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync("http://localhost:5002/schedule/add", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SlotResponse>(json);
            Log.Information("Слот добавлен для врача {DoctorId} на {Time}", 
                slotRequest.DoctorId, slotRequest.Time);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при добавлении слота");
            throw;
        }
    }
}

public interface IAppointmentAdapter
{
    Task<AppointmentResponse> CreateAppointmentAsync(AppointmentRequest appointmentRequest);
}

public class AppointmentAdapter : IAppointmentAdapter
{
    private readonly HttpClient _httpClient;
    public AppointmentAdapter(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AppointmentResponse> CreateAppointmentAsync(AppointmentRequest appointmentRequest)
    {
        try
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(appointmentRequest),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync("http://localhost:3000/appointment/create", content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AppointmentResponse>(json);
            Log.Information("Запись создана для пациента {PatientId} к врачу {DoctorId}",
                appointmentRequest.PatientId, appointmentRequest.DoctorId);
            return result;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ошибка при создании записи");
            throw;
        }
    }
}
