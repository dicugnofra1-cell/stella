using Mandorle.Application.GoodsReceipts.Models;
using MediatR;

namespace Mandorle.Application.GoodsReceipts.Commands;

public record UpdateGoodsReceiptCommand(
    int BatchId,
    int? SupplierId,
    RegisterGoodsReceiptSupplierInput? NewSupplier,
    int? ProductId,
    RegisterGoodsReceiptProductInput? NewProduct,
    string BatchType,
    string? Variety,
    decimal Quantity,
    decimal PurchaseUnitPrice,
    string UnitOfMeasure,
    bool BioFlag,
    int? SupplierDocumentId,
    int? CertificationId,
    DateTime? ReceivedAt,
    DateOnly? ProductionDate,
    DateOnly? ExpirationDate,
    string? Notes,
    string UserId) : IRequest<GoodsReceiptDto>;
