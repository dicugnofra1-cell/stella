using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record GetSupplierByVatNumberQuery(string VatNumber) : IRequest<SupplierDto?>;
