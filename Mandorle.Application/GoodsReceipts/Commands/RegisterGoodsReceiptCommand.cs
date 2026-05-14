using Mandorle.Application.GoodsReceipts.Models;
using MediatR;

namespace Mandorle.Application.GoodsReceipts.Commands;

public record RegisterGoodsReceiptCommand(
    int? SupplierId,
    RegisterGoodsReceiptSupplierInput? NewSupplier,
    int? ProductId,
    RegisterGoodsReceiptProductInput? NewProduct,
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

public record RegisterGoodsReceiptSupplierInput(
    string Name,
    string? VatNumber,
    string? Address,
    string? Email,
    string? Phone,
    string? Status);

public record RegisterGoodsReceiptProductInput(
    string Name,
    string? Sku,
    string? Description,
    string UnitOfMeasure,
    string? Category,
    bool? ChannelB2BEnabled,
    bool? ChannelB2CEnabled,
    bool? Active);
