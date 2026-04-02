using Mandorle.Application.Customers.Models;
using MediatR;

namespace Mandorle.Application.Customers.Queries;

public record SearchCustomersQuery(string? Search, string? Type, string? Status) : IRequest<IReadOnlyList<CustomerDto>>;
