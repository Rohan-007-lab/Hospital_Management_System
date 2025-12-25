using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<Medicine> Medicines { get; set; }
    public DbSet<PrescriptionMedicine> PrescriptionMedicines { get; set; }
    public DbSet<LabTest> LabTests { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillItem> BillItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<Bed> Beds { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.Address).HasMaxLength(500);

            // Soft delete query filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Patient Configuration
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientNumber).IsUnique();
            entity.Property(e => e.PatientNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.MedicalHistory).HasMaxLength(2000);
            entity.Property(e => e.Allergies).HasMaxLength(500);
            entity.Property(e => e.EmergencyContactName).HasMaxLength(100);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(15);

            entity.HasOne(e => e.User)
                .WithOne(e => e.Patient)
                .HasForeignKey<Patient>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Doctor Configuration
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.DoctorNumber).IsUnique();
            entity.Property(e => e.DoctorNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Specialization).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Qualifications).HasMaxLength(500);
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.ConsultationFee).HasPrecision(18, 2);

            entity.HasOne(e => e.User)
                .WithOne(e => e.Doctor)
                .HasForeignKey<Doctor>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Appointment Configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AppointmentNumber).IsUnique();
            entity.Property(e => e.AppointmentNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ReasonForVisit).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Symptoms).HasMaxLength(1000);
            entity.Property(e => e.DoctorNotes).HasMaxLength(2000);
            entity.Property(e => e.Diagnosis).HasMaxLength(1000);

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Doctor)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Prescription Configuration
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PrescriptionNumber).IsUnique();
            entity.Property(e => e.PrescriptionNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Diagnosis).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.GeneralInstructions).HasMaxLength(2000);

            entity.HasOne(e => e.Appointment)
                .WithOne(e => e.Prescription)
                .HasForeignKey<Prescription>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Doctor)
                .WithMany(e => e.Prescriptions)
                .HasForeignKey(e => e.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Medicine Configuration
        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MedicineName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.GenericName).HasMaxLength(200);
            entity.Property(e => e.Manufacturer).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // PrescriptionMedicine Configuration
        modelBuilder.Entity<PrescriptionMedicine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Dosage).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Frequency).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Instructions).HasMaxLength(500);

            entity.HasOne(e => e.Prescription)
                .WithMany(e => e.PrescriptionMedicines)
                .HasForeignKey(e => e.PrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Medicine)
                .WithMany(e => e.PrescriptionMedicines)
                .HasForeignKey(e => e.MedicineId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // LabTest Configuration
        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TestNumber).IsUnique();
            entity.Property(e => e.TestNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TestName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TestType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TestPrice).HasPrecision(18, 2);
            entity.Property(e => e.Results).HasMaxLength(2000);

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.LabTests)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Bill Configuration
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BillNumber).IsUnique();
            entity.Property(e => e.BillNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.Discount).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
            entity.Property(e => e.BalanceAmount).HasPrecision(18, 2);

            entity.HasOne(e => e.Patient)
                .WithMany(e => e.Bills)
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // BillItem Configuration
        modelBuilder.Entity<BillItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalPrice).HasPrecision(18, 2);

            entity.HasOne(e => e.Bill)
                .WithMany(e => e.BillItems)
                .HasForeignKey(e => e.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Payment Configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Bill)
                .WithMany(e => e.Payments)
                .HasForeignKey(e => e.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Ward Configuration
        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WardName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.WardType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Floor).HasMaxLength(20);
            entity.Property(e => e.ChargesPerDay).HasPrecision(18, 2);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Bed Configuration
        modelBuilder.Entity<Bed>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BedNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(e => e.Ward)
                .WithMany(e => e.Beds)
                .HasForeignKey(e => e.WardId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Seed default admin user
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Admin User
        var adminUser = new User
        {
            Id = 1,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@hospital.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            PhoneNumber = "9999999999",
            DateOfBirth = new DateTime(1990, 1, 1),
            Gender = HMS.Domain.Enums.Gender.Male,
            Address = "Hospital Address",
            Role = HMS.Domain.Enums.UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        modelBuilder.Entity<User>().HasData(adminUser);

        // Sample Medicines
        var medicines = new List<Medicine>
    {
        new Medicine { Id = 1, MedicineName = "Paracetamol", GenericName = "Acetaminophen", Manufacturer = "ABC Pharma", Category = "Pain Relief", UnitPrice = 10.00M, StockQuantity = 500, ReorderLevel = 100, CreatedAt = DateTime.UtcNow },
        new Medicine { Id = 2, MedicineName = "Amoxicillin", GenericName = "Amoxicillin", Manufacturer = "XYZ Pharma", Category = "Antibiotic", UnitPrice = 50.00M, StockQuantity = 300, ReorderLevel = 50, CreatedAt = DateTime.UtcNow },
        new Medicine { Id = 3, MedicineName = "Ibuprofen", GenericName = "Ibuprofen", Manufacturer = "DEF Pharma", Category = "Pain Relief", UnitPrice = 15.00M, StockQuantity = 400, ReorderLevel = 80, CreatedAt = DateTime.UtcNow }
    };

        modelBuilder.Entity<Medicine>().HasData(medicines);
    }
}