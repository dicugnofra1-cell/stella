using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record GetSupplierByIdQuery(int Id) : IRequest<SupplierDto?>;
