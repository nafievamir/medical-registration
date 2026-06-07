using Polly;
using Polly.CircuitBreaker;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient();

        var app = builder.Build();
        var logger = app.Logger;

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var doctors = new List<Doctor>();
        var slots = new List<Slot>();
        var appointments = new List<Appointment>();

        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (exception, timeSpan, retryCount, context) =>
            {
                logger.LogWarning("Повторная попытка {RetryCount} через {TimeSpan} сек. Ошибка: {Error}", 
                    retryCount, timeSpan.TotalSeconds, exception.Message);
            });

        var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
            onBreak: (exception, duration) =>
            {
                logger.LogError("Circuit Breaker АКТИВИРОВАН на {Duration} сек. Ошибка: {Error}", duration.TotalSeconds, exception.Message);
            },
            onReset: () =>
            {
                logger.LogInformation("Circuit Breaker СБРОШЕН");
            });

        app.MapGet("/api/doctors", () =>
        {
            logger.LogInformation("GET /api/doctors - получено {Count} врачей", doctors.Count);
            return doctors;
        });

        app.MapPost("/api/doctors", (Doctor doctor) =>
        {
            doctor.Id = doctors.Count + 1;
            doctors.Add(doctor);
            logger.LogInformation("POST /api/doctors - создан врач ID:{DoctorId} - {Name} ({Specialty})", 
                doctor.Id, doctor.Name, doctor.Specialty);
            return Results.Created($"/api/doctors/{doctor.Id}", doctor);
        });

        app.MapPost("/api/schedule/add", (Slot slot) =>
        {
            slot.Id = slots.Count + 1;
            slots.Add(slot);
            logger.LogInformation("POST /api/schedule/add - слот ID:{SlotId} для врача ID:{DoctorId} на {Time}", 
                slot.Id, slot.DoctorId, slot.Time);
            return Results.Ok(new { status = "added" });
        });

        app.MapPost("/api/appointment/create", async (Appointment appointment, HttpClient client) =>
        {
            logger.LogInformation("POST /api/appointment/create - создание записи для пациента {PatientId} к врачу {DoctorId}", 
                appointment.PatientId, appointment.DoctorId);

            var doctor = doctors.FirstOrDefault(d => d.Id == appointment.DoctorId);
            if (doctor == null)
            {
                logger.LogError("Врач ID:{DoctorId} не найден", appointment.DoctorId);
                return Results.BadRequest(new { error = "Doctor not found" });
            }

            try
            {
                var result = await retryPolicy.ExecuteAsync(async () =>
                {
                    logger.LogInformation("Вызов внешнего API...");
                    var response = await client.GetAsync("https://httpbin.org/status/500");
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при вызове внешнего API после всех попыток, но запись всё равно создаётся");
            }

            appointment.Id = appointments.Count + 1;
            appointment.Status = "confirmed";
            appointments.Add(appointment);
            
            logger.LogInformation("Запись создана успешно! ID:{AppointmentId}", appointment.Id);
            return Results.Ok(new { id = appointment.Id, status = appointment.Status });
        });

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.Run();
    }
}

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}

public class Slot
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public string Time { get; set; } = string.Empty;
}

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string DateTime { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
}
