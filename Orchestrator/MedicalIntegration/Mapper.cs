namespace MedicalIntegration;

public static class DataMapper
{
    public static AppointmentRequest MapToAppointmentRequest(int patientId, Doctor doctor, string datetime)
    {
        return new AppointmentRequest
        {
            PatientId = patientId,
            DoctorId = doctor.Id,
            Datetime = datetime
        };
    }

    public static SlotRequest MapToSlotRequest(Doctor doctor, string time)
    {
        return new SlotRequest
        {
            DoctorId = doctor.Id,
            Time = time
        };
    }
}
