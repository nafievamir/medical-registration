using MedicalIntegration;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/integration.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Запуск интеграционного сервиса");

    // Настройка HttpClient'ов
    var httpClient = new HttpClient();
    var doctorsAdapter = new DoctorsAdapter(httpClient);
    var scheduleAdapter = new ScheduleAdapter(httpClient);
    var appointmentAdapter = new AppointmentAdapter(httpClient);

    var orchestrator = new OrchestratorService(
        doctorsAdapter,
        scheduleAdapter,
        appointmentAdapter
    );

    // Пример вызова оркестрации
    await orchestrator.CreateFullAppointmentAsync(
        patientId: 1,
        doctorId: 1,
        datetime: "2025-06-07T10:00:00",
        slotTime: "10:00"
    );

    Log.Information("Программа завершена успешно");
}
catch (Exception ex)
{
    Log.Error(ex, "Критическая ошибка");
}
finally
{
    Log.CloseAndFlush();
}
