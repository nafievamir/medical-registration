using Xunit;

namespace MedicalWeb.Tests;

public class UnitTests
{
    [Fact]
    public void CreateDoctor_Should_SetCorrectProperties()
    {
        var doctor = new Doctor { Name = "Иванов И.И.", Specialty = "Терапевт" };
        doctor.Id = 1;
        Assert.Equal("Иванов И.И.", doctor.Name);
        Assert.Equal("Терапевт", doctor.Specialty);
        Assert.Equal(1, doctor.Id);
    }

    [Fact]
    public void CreateSlot_Should_LinkToDoctor()
    {
        var slot = new Slot { DoctorId = 1, Time = "10:00" };
        Assert.Equal(1, slot.DoctorId);
        Assert.Equal("10:00", slot.Time);
    }
}
