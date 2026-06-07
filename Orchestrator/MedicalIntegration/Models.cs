namespace MedicalIntegration;

public class Doctor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Specialty { get; set; }
}

public class SlotRequest
{
    public int DoctorId { get; set; }
    public string Time { get; set; }
}

public class SlotResponse
{
    public string Status { get; set; }
    public Slot Slot { get; set; }
}

public class Slot
{
    public int? SlotId { get; set; }
    public int? DoctorId { get; set; }
    public string Time { get; set; }
}

public class AppointmentRequest
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Datetime { get; set; }
}

public class AppointmentResponse
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Datetime { get; set; }
    public string Status { get; set; }
}
