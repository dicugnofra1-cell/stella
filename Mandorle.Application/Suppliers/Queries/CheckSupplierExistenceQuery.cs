using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record CheckSupplierExistenceQuery(
    string? Email,
    string? VatNumber,
    int? ExcludeSupplierId) : IRequest<SupplierExistenceCheckResultDto>;
