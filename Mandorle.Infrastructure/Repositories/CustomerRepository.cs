using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly StellaFruttaDbContext _context;

    public CustomerRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public Task<Customer?> GetByIdAsync(int id, bool includeAddresses = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> query = _context.Customers;

        if (includeAddresses)
        {
            query = query.Include(customer => customer.CustomerAddresses);
        }

        return query.FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
    }

    public Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(email);

        return _context.Customers
            .FirstOrDefaultAsync(customer => customer.Email == normalizedEmail, cancellationToken);
    }

    public Task<Customer?> GetByPecAsync(string pec, CancellationToken cancellationToken = default)
    {
        var normalizedPec = Normalize(pec);

        return _context.Customers
            .FirstOrDefaultAsync(customer => customer.Pec == normalizedPec, cancellationToken);
    }

    public Task<Customer?> GetBySpidIdentifierAsync(string spidIdentifier, CancellationToken cancellationToken = default)
    {
        var normalizedSpidIdentifier = Normalize(spidIdentifier);

        return _context.Customers
            .FirstOrDefaultAsync(customer => customer.SpidIdentifier == normalizedSpidIdentifier, cancellationToken);
    }

    public Task<Customer?> GetByVatNumberAsync(string vatNumber, CancellationToken cancellationToken = default)
    {
        var normalizedVatNumber = Normalize(vatNumber);

        return _context.Customers
            .FirstOrDefaultAsync(customer => customer.VatNumber == normalizedVatNumber, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, int? excludeCustomerId = null, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(email);

        return _context.Customers.AnyAsync(
            customer => customer.Email == normalizedEmail && (!excludeCustomerId.HasValue || customer.Id != excludeCustomerId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsByPecAsync(string? pec, int? excludeCustomerId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(pec))
        {
            return Task.FromResult(false);
        }

        var normalizedPec = Normalize(pec);

        return _context.Customers.AnyAsync(
            customer => customer.Pec == normalizedPec && (!excludeCustomerId.HasValue || customer.Id != excludeCustomerId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsBySpidIdentifierAsync(string? spidIdentifier, int? excludeCustomerId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(spidIdentifier))
        {
            return Task.FromResult(false);
        }

        var normalizedSpidIdentifier = Normalize(spidIdentifier);

        return _context.Customers.AnyAsync(
            customer => customer.SpidIdentifier == normalizedSpidIdentifier && (!excludeCustomerId.HasValue || customer.Id != excludeCustomerId.Value),
            cancellationToken);
    }

    public Task<bool> ExistsByVatNumberAsync(string? vatNumber, int? excludeCustomerId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(vatNumber))
        {
            return Task.FromResult(false);
        }

        var normalizedVatNumber = Normalize(vatNumber);

        return _context.Customers.AnyAsync(
            customer => customer.VatNumber == normalizedVatNumber && (!excludeCustomerId.HasValue || customer.Id != excludeCustomerId.Value),
            cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> SearchAsync(string? search, string? type, string? status, CancellationToken cancellationToken = default)
    {
        var query = _context.Customers.AsNoTracking().AsQueryable();
        var normalizedSearch = Normalize(search);
        var normalizedType = Normalize(type);
        var normalizedStatus = Normalize(status);

        if (!string.IsNullOrWhiteSpace(normalizedType))
        {
            query = query.Where(customer => customer.Type == normalizedType);
        }

        if (!string.IsNullOrWhiteSpace(normalizedStatus))
        {
            query = query.Where(customer => customer.Status == normalizedStatus);
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(customer =>
                customer.Name.Contains(normalizedSearch) ||
                customer.Email.Contains(normalizedSearch) ||
                (customer.Pec != null && customer.Pec.Contains(normalizedSearch)) ||
                (customer.VatNumber != null && customer.VatNumber.Contains(normalizedSearch)) ||
                (customer.SpidIdentifier != null && customer.SpidIdentifier.Contains(normalizedSearch)));
        }

        return await query
            .OrderBy(customer => customer.Name)
            .ThenBy(customer => customer.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerAddress>> GetAddressesAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _context.CustomerAddresses
            .AsNoTracking()
            .Where(address => address.CustomerId == customerId)
            .OrderByDescending(address => address.IsDefault)
            .ThenBy(address => address.AddressType)
            .ThenBy(address => address.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<CustomerAddress?> GetAddressByIdAsync(int customerId, int addressId, CancellationToken cancellationToken = default)
    {
        return _context.CustomerAddresses
            .FirstOrDefaultAsync(address => address.CustomerId == customerId && address.Id == addressId, cancellationToken);
    }

    public Task<CustomerAddress?> GetDefaultAddressByTypeAsync(int customerId, string addressType, CancellationToken cancellationToken = default)
    {
        var normalizedAddressType = Normalize(addressType);

        return _context.CustomerAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(
                address => address.CustomerId == customerId && address.AddressType == normalizedAddressType && address.IsDefault,
                cancellationToken);
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        NormalizeCustomer(customer);
        return _context.Customers.AddAsync(customer, cancellationToken).AsTask();
    }

    public Task AddAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default)
    {
        NormalizeAddress(address);
        return _context.CustomerAddresses.AddAsync(address, cancellationToken).AsTask();
    }

    public void Update(Customer customer)
    {
        NormalizeCustomer(customer);
        _context.Customers.Update(customer);
    }

    public void UpdateAddress(CustomerAddress address)
    {
        NormalizeAddress(address);
        _context.CustomerAddresses.Update(address);
    }

    public async Task SetDefaultAddressAsync(int customerId, int addressId, CancellationToken cancellationToken = default)
    {
        var targetAddress = await _context.CustomerAddresses
            .FirstOrDefaultAsync(address => address.CustomerId == customerId && address.Id == addressId, cancellationToken);

        if (targetAddress is null)
        {
            return;
        }

        var relatedAddresses = await _context.CustomerAddresses
            .Where(address => address.CustomerId == customerId && address.AddressType == targetAddress.AddressType)
            .ToListAsync(cancellationToken);

        foreach (var address in relatedAddresses)
        {
            address.IsDefault = address.Id == addressId;
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    private static void NormalizeCustomer(Customer customer)
    {
        customer.Type = Normalize(customer.Type)!;
        customer.Name = Normalize(customer.Name)!;
        customer.Email = Normalize(customer.Email)!;
        customer.Pec = Normalize(customer.Pec);
        customer.SpidIdentifier = Normalize(customer.SpidIdentifier);
        customer.Phone = Normalize(customer.Phone);
        customer.VatNumber = Normalize(customer.VatNumber);
        customer.Status = Normalize(customer.Status)!;
    }

    private static void NormalizeAddress(CustomerAddress address)
    {
        address.AddressType = Normalize(address.AddressType)!;
        address.RecipientName = Normalize(address.RecipientName)!;
        address.Street = Normalize(address.Street)!;
        address.Street2 = Normalize(address.Street2);
        address.City = Normalize(address.City)!;
        address.PostalCode = Normalize(address.PostalCode)!;
        address.Province = Normalize(address.Province)!;
        address.Country = Normalize(address.Country)!;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
