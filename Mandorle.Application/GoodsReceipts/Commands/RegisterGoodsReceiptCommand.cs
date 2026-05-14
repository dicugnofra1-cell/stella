using Mandorle.Application.GoodsReceipts.Models;
using MediatR;

namespace Mandorle.Application.GoodsReceipts.Commands;

public record RegisterGoodsReceiptCommand(
    int SupplierId,
    int ProductId,
    string BatchType,
    string? Variety,
    decimal Quantity,
    string UnitOfMeasure,
    bool BioFlag,
    int? SupplierDocumentId,
    int? CertificationId,
    DateTime? ReceivedAt,
    DateOnly? ProductionDate,
    DateOnly? ExpirationDate,
    string? Notes,
    string UserId) : IRequest<GoodsReceiptDto>;
