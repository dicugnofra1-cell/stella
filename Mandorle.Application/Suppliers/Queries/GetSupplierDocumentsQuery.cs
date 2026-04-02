using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record GetSupplierDocumentsQuery(int SupplierId) : IRequest<IReadOnlyList<SupplierDocumentDto>>;
