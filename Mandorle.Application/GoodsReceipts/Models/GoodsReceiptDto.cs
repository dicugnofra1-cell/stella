using Mandorle.Application.Batches.Models;
using Mandorle.Application.Inventory.Models;

namespace Mandorle.Application.GoodsReceipts.Models;

public record GoodsReceiptDto(
    BatchDto Batch,
    InventoryMovementDto InventoryMovement,
    decimal InitialQuantity,
    string UnitOfMeasure,
    int? SupplierDocumentId,
    int? CertificationId);
