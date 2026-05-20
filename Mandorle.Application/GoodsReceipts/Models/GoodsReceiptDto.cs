using Mandorle.Application.Batches.Models;
using Mandorle.Application.Inventory.Models;

namespace Mandorle.Application.GoodsReceipts.Models;

public record GoodsReceiptDto(
    BatchDto Batch,
    InventoryMovementDto InventoryMovement,
    decimal InitialQuantity,
    decimal PurchaseUnitPrice,
    string UnitOfMeasure,
    int? SupplierDocumentId,
    int? CertificationId);
