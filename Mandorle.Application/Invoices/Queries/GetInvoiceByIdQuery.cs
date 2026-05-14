using Mandorle.Application.Invoices.Models;
using MediatR;

namespace Mandorle.Application.Invoices.Queries;

public record GetInvoiceByIdQuery(int Id) : IRequest<InvoiceDto?>;
