using Serilog;

namespace MedicalIntegration;

public class OrchestratorService
{
    private readonly IDoctorsAdapter _doctorsAdapter;
    private readonly IScheduleAdapter _scheduleAdapter;
    private readonly IAppointmentAdapter _appointmentAdapter;

    public OrchestratorService(
        IDoctorsAdapter doctorsAdapter,
        IScheduleAdapter scheduleAdapter,
        IAppointmentAdapter appointmentAdapter)
    {
        _doctorsAdapter = doctorsAdapter;
        _scheduleAdapter = scheduleAdapter;
        _appointmentAdapter = appointmentAdapter;
    }

    public async Task CreateFullAppointmentAsync(int patientId, int doctorId, string datetime, string slotTime)
    {
        Log.Information("Начало интеграции: запись пациента {PatientId} ко врачу {DoctorId}", patientId, doctorId);

        // 1. Получаем список врачей (модуль А)
        var doctors = await _doctorsAdapter.GetDoctorsAsync();
        var doctor = doctors.FirstOrDefault(d => d.Id == doctorId);
        if (doctor == null)
        {
            Log.Error("Врач с ID {DoctorId} не найден", doctorId);
            throw new Exception($"Doctor {doctorId} not found");
        }
        Log.Information("Найден врач: {DoctorName} ({Specialty})", doctor.Name, doctor.Specialty);

        // 2. Добавляем слот в расписание (модуль Б)
        var slotRequest = DataMapper.MapToSlotRequest(doctor, slotTime);
        var slotResult = await _scheduleAdapter.AddSlotAsync(slotRequest);
        Log.Information("Результат добавления слота: {Status}", slotResult?.Status ?? "unknown");

        // 3. Создаём запись (модуль В)
        var appointmentRequest = DataMapper.MapToAppointmentRequest(patientId, doctor, datetime);
        var appointmentResult = await _appointmentAdapter.CreateAppointmentAsync(appointmentRequest);
        Log.Information("Запись создана, ID: {AppointmentId}, статус: {Status}",
            appointmentResult?.Id, appointmentResult?.Status);

        Log.Information("Интеграция успешно завершена");
    }
}
