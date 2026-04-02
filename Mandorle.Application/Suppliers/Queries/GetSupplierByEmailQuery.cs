using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record GetSupplierByEmailQuery(string Email) : IRequest<SupplierDto?>;
