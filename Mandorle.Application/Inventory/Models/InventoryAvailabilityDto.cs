namespace Mandorle.Application.Inventory.Models;

public record InventoryAvailabilityDto(
    int? ProductId,
    int? BatchId,
    decimal PhysicalStock,
    decimal ReservedStock,
    decimal AvailableStock);
