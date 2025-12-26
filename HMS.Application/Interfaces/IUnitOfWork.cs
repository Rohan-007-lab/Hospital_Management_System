using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Domain.Entities;

namespace HMS.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<User> Users { get; }
    IGenericRepository<Patient> Patients { get; }
    IGenericRepository<Doctor> Doctors { get; }
    IGenericRepository<Appointment> Appointments { get; }
    IGenericRepository<Prescription> Prescriptions { get; }
    IGenericRepository<Medicine> Medicines { get; }
    IGenericRepository<PrescriptionMedicine> PrescriptionMedicines { get; }
    IGenericRepository<LabTest> LabTests { get; }
    IGenericRepository<Bill> Bills { get; }
    IGenericRepository<BillItem> BillItems { get; }
    IGenericRepository<Payment> Payments { get; }
    IGenericRepository<Ward> Wards { get; }
    IGenericRepository<Bed> Beds { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    void Dispose();
}