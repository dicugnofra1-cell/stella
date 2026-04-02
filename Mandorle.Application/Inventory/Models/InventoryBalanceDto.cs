namespace Mandorle.Application.Inventory.Models;

public record InventoryBalanceDto(
    int? ProductId,
    int? BatchId,
    decimal Balance);
