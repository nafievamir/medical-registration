using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite("Data Source=medical.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();
var logger = app.Logger;

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/doctors", async (AppDbContext db) =>
{
    var doctors = await db.Doctors.ToListAsync();
    return Results.Ok(doctors);
});

app.MapPost("/api/doctors", async (Doctor doctor, AppDbContext db) =>
{
    await db.Doctors.AddAsync(doctor);
    await db.SaveChangesAsync();
    logger.LogInformation("Врач создан ID:{DoctorId}", doctor.Id);
    return Results.Created($"/api/doctors/{doctor.Id}", doctor);
});

app.MapGet("/api/slots", async (AppDbContext db) => await db.Slots.ToListAsync());

app.MapPost("/api/schedule/add", async (Slot slot, AppDbContext db) =>
{
    await db.Slots.AddAsync(slot);
    await db.SaveChangesAsync();
    return Results.Ok(new { status = "added" });
});

app.MapGet("/api/appointments", async (AppDbContext db) => await db.Appointments.ToListAsync());

app.MapPost("/api/appointment/create", async (Appointment appointment, AppDbContext db) =>
{
    var doctor = await db.Doctors.FindAsync(appointment.DoctorId);
    if (doctor == null) return Results.BadRequest(new { error = "Doctor not found" });
    
    await db.Appointments.AddAsync(appointment);
    await db.SaveChangesAsync();
    return Results.Ok(new { id = appointment.Id, status = "confirmed" });
});

app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();

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

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
}
