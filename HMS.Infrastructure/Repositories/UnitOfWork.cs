using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HMS.Application.Interfaces;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace HMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public IGenericRepository<User> Users { get; }
    public IGenericRepository<Patient> Patients { get; }
    public IGenericRepository<Doctor> Doctors { get; }
    public IGenericRepository<Appointment> Appointments { get; }
    public IGenericRepository<Prescription> Prescriptions { get; }
    public IGenericRepository<Medicine> Medicines { get; }
    public IGenericRepository<PrescriptionMedicine> PrescriptionMedicines { get; }
    public IGenericRepository<LabTest> LabTests { get; }
    public IGenericRepository<Bill> Bills { get; }
    public IGenericRepository<BillItem> BillItems { get; }
    public IGenericRepository<Payment> Payments { get; }
    public IGenericRepository<Ward> Wards { get; }
    public IGenericRepository<Bed> Beds { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Users = new GenericRepository<User>(_context);
        Patients = new GenericRepository<Patient>(_context);
        Doctors = new GenericRepository<Doctor>(_context);
        Appointments = new GenericRepository<Appointment>(_context);
        Prescriptions = new GenericRepository<Prescription>(_context);
        Medicines = new GenericRepository<Medicine>(_context);
        PrescriptionMedicines = new GenericRepository<PrescriptionMedicine>(_context);
        LabTests = new GenericRepository<LabTest>(_context);
        Bills = new GenericRepository<Bill>(_context);
        BillItems = new GenericRepository<BillItem>(_context);
        Payments = new GenericRepository<Payment>(_context);
        Wards = new GenericRepository<Ward>(_context);
        Beds = new GenericRepository<Bed>(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}