using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record SearchSuppliersQuery(string? Search, string? Status) : IRequest<IReadOnlyList<SupplierDto>>;
