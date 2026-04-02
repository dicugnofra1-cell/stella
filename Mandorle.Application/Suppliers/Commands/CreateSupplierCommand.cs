using Mandorle.Application.Suppliers.Models;
using MediatR;

namespace Mandorle.Application.Suppliers.Commands;

public record CreateSupplierCommand(
    string Name,
    string? VatNumber,
    string? Address,
    string? Email,
    string? Phone,
    string Status) : IRequest<SupplierDto>;
