using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly StellaFruttaDbContext _context;

    public SupplierRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Suppliers.FirstOrDefaultAsync(supplier => supplier.Id == id, cancellationToken);
    }

    public Task<Supplier?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(email);
        return _context.Suppliers.FirstOrDefaultAsync(supplier => supplier.Email == normalizedEmail, cancellationToken);
    }

    public Task<Supplier?> GetByVatNumberAsync(string vatNumber, CancellationToken cancellationToken = default)
    {
        var normalizedVatNumber = Normalize(vatNumber);
        return _context.Suppliers.FirstOrDefaultAsync(supplier => supplier.VatNumber == normalizedVatNumber, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string? email, int? excludeSupplierId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult(false);
        }

        var normalizedEmail = Normalize(email);

        return _context.Suppliers.AnyAsync(
            supplier => supplier.Email == normalizedEmail && (!excludeSupplierId.HasValue || supplier.Id != excludeSupplierId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsByVatNumberAsync(string? vatNumber, int? excludeSupplierId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(vatNumber))
        {
            return Task.FromResult(false);
        }

        var normalizedVatNumber = Normalize(vatNumber);

        return _context.Suppliers.AnyAsync(
            supplier => supplier.VatNumber == normalizedVatNumber && (!excludeSupplierId.HasValue || supplier.Id != excludeSupplierId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Supplier>> SearchAsync(string? search, string? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Suppliers.AsNoTracking().AsQueryable();
        var normalizedSearch = Normalize(search);
        var normalizedStatus = Normalize(status);

        if (!string.IsNullOrWhiteSpace(normalizedStatus))
        {
            query = query.Where(supplier => supplier.Status == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(supplier =>
                supplier.Name.Contains(normalizedSearch) ||
                (supplier.Email != null && supplier.Email.Contains(normalizedSearch)) ||
                (supplier.VatNumber != null && supplier.VatNumber.Contains(normalizedSearch)) ||
                (supplier.Address != null && supplier.Address.Contains(normalizedSearch)));
        }

        return await query
            .OrderBy(supplier => supplier.Name)
            .ThenBy(supplier => supplier.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        NormalizeSupplier(supplier);
        return _context.Suppliers.AddAsync(supplier, cancellationToken).AsTask();
    }

    public void Update(Supplier supplier)
    {
        NormalizeSupplier(supplier);
        _context.Suppliers.Update(supplier);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeSupplier(Supplier supplier)
    {
        supplier.Name = Normalize(supplier.Name)!;
        supplier.VatNumber = Normalize(supplier.VatNumber);
        supplier.Address = Normalize(supplier.Address);
        supplier.Email = Normalize(supplier.Email);
        supplier.Phone = Normalize(supplier.Phone);
        supplier.Status = Normalize(supplier.Status)!;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
