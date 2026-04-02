using Mandorle.Domain.Entities;

namespace Mandorle.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id, bool includeAddresses = false, CancellationToken cancellationToken = default);

    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Customer?> GetByPecAsync(string pec, CancellationToken cancellationToken = default);

    Task<Customer?> GetBySpidIdentifierAsync(string spidIdentifier, CancellationToken cancellationToken = default);

    Task<Customer?> GetByVatNumberAsync(string vatNumber, CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailAsync(string email, int? excludeCustomerId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsByPecAsync(string? pec, int? excludeCustomerId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySpidIdentifierAsync(string? spidIdentifier, int? excludeCustomerId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsByVatNumberAsync(string? vatNumber, int? excludeCustomerId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> SearchAsync(string? search, string? type, string? status, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CustomerAddress>> GetAddressesAsync(int customerId, CancellationToken cancellationToken = default);

    Task<CustomerAddress?> GetAddressByIdAsync(int customerId, int addressId, CancellationToken cancellationToken = default);

    Task<CustomerAddress?> GetDefaultAddressByTypeAsync(int customerId, string addressType, CancellationToken cancellationToken = default);

    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);

    Task AddAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default);

    void Update(Customer customer);

    void UpdateAddress(CustomerAddress address);

    Task SetDefaultAddressAsync(int customerId, int addressId, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
