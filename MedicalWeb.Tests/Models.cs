namespace MedicalWeb.Tests;

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
    public string Status { get; set; } = string.Empty;
}
