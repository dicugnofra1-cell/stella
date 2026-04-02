using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class CertificationRepository : ICertificationRepository
{
    private readonly StellaFruttaDbContext _context;

    public CertificationRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<Certification?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Certifications.FirstOrDefaultAsync(certification => certification.Id == id, cancellationToken);
    }

    public Task<Certification?> GetByIdAsync(int supplierId, int id, CancellationToken cancellationToken = default)
    {
        return _context.Certifications.FirstOrDefaultAsync(
            certification => certification.SupplierId == supplierId && certification.Id == id,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Certification>> GetBySupplierIdAsync(
        int supplierId,
        string? type = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Certifications
            .AsNoTracking()
            .Where(certification => certification.SupplierId == supplierId);

        var normalizedType = Normalize(type);
        var normalizedStatus = Normalize(status);

        if (!string.IsNullOrWhiteSpace(normalizedType))
        {
            query = query.Where(certification => certification.Type == normalizedType);
        }

        if (!string.IsNullOrWhiteSpace(normalizedStatus))
        {
            query = query.Where(certification => certification.Status == normalizedStatus);
        }

        return await query
            .OrderByDescending(certification => certification.ExpiryDate)
            .ThenByDescending(certification => certification.Id)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Certification certification, CancellationToken cancellationToken = default)
    {
        NormalizeCertification(certification);
        return _context.Certifications.AddAsync(certification, cancellationToken).AsTask();
    }

    public void Update(Certification certification)
    {
        NormalizeCertification(certification);
        _context.Certifications.Update(certification);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeCertification(Certification certification)
    {
        certification.Type = Normalize(certification.Type)!;
        certification.Authority = Normalize(certification.Authority)!;
        certification.Code = Normalize(certification.Code);
        certification.Status = Normalize(certification.Status)!;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
