using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Queries;

public record GetSupplierDocumentByIdQuery(int SupplierId, int DocumentId) : IRequest<SupplierDocumentDto?>;
