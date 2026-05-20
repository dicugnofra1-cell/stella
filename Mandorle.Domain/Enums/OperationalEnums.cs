namespace Mandorle.Domain.Enums;

public enum BatchStatus
{
    Ricevuto,
    InLavorazione,
    Disponibile,
    Confezionato,
    ParzialmenteVenduto,
    Esaurito,
    Bloccato
}

public enum InventoryMovementType
{
    Load,
    Unload,
    ReturnIn,
    ReturnOut,
    Waste,
    TransferOut,
    AdjustmentIn,
    AdjustmentOut
}

public enum StockReservationStatus
{
    Active,
    Reserved,
    Pending,
    Released,
    Consumed
}

public enum StockReservationType
{
    Order
}

public enum OrderStatus
{
    Confermato,
    Spedito,
    Evaso,
    Confirmed,
    Shipped,
    Completed
}

public enum CertificationStatus
{
    Active
}

public enum MovementReferenceType
{
    GoodsReceipt,
    Order,
    Processing
}

public enum InvoiceDocumentType
{
    Fattura,
    Ddt
}

public static class OperationalEnumMappings
{
    private static readonly IReadOnlyDictionary<string, BatchStatus> BatchStatuses = new Dictionary<string, BatchStatus>(StringComparer.OrdinalIgnoreCase)
    {
        ["RICEVUTO"] = BatchStatus.Ricevuto,
        ["IN_LAVORAZIONE"] = BatchStatus.InLavorazione,
        ["DISPONIBILE"] = BatchStatus.Disponibile,
        ["CONFEZIONATO"] = BatchStatus.Confezionato,
        ["PARZIALMENTE_VENDUTO"] = BatchStatus.ParzialmenteVenduto,
        ["ESAURITO"] = BatchStatus.Esaurito,
        ["BLOCCATO"] = BatchStatus.Bloccato
    };

    private static readonly IReadOnlyDictionary<string, InventoryMovementType> InventoryMovementTypes = new Dictionary<string, InventoryMovementType>(StringComparer.OrdinalIgnoreCase)
    {
        ["LOAD"] = InventoryMovementType.Load,
        ["UNLOAD"] = InventoryMovementType.Unload,
        ["RETURN_IN"] = InventoryMovementType.ReturnIn,
        ["RETURN_OUT"] = InventoryMovementType.ReturnOut,
        ["WASTE"] = InventoryMovementType.Waste,
        ["TRANSFER_OUT"] = InventoryMovementType.TransferOut,
        ["ADJUSTMENT_IN"] = InventoryMovementType.AdjustmentIn,
        ["ADJUSTMENT_OUT"] = InventoryMovementType.AdjustmentOut
    };

    private static readonly IReadOnlyDictionary<string, StockReservationStatus> StockReservationStatuses = new Dictionary<string, StockReservationStatus>(StringComparer.OrdinalIgnoreCase)
    {
        ["ACTIVE"] = StockReservationStatus.Active,
        ["RESERVED"] = StockReservationStatus.Reserved,
        ["PENDING"] = StockReservationStatus.Pending,
        ["RELEASED"] = StockReservationStatus.Released,
        ["CONSUMED"] = StockReservationStatus.Consumed
    };

    private static readonly IReadOnlyDictionary<string, OrderStatus> OrderStatuses = new Dictionary<string, OrderStatus>(StringComparer.OrdinalIgnoreCase)
    {
        ["CONFERMATO"] = OrderStatus.Confermato,
        ["SPEDITO"] = OrderStatus.Spedito,
        ["EVASO"] = OrderStatus.Evaso,
        ["CONFIRMED"] = OrderStatus.Confirmed,
        ["SHIPPED"] = OrderStatus.Shipped,
        ["COMPLETED"] = OrderStatus.Completed
    };

    public static string ToDbValue(this BatchStatus status) => status switch
    {
        BatchStatus.Ricevuto => "RICEVUTO",
        BatchStatus.InLavorazione => "IN_LAVORAZIONE",
        BatchStatus.Disponibile => "DISPONIBILE",
        BatchStatus.Confezionato => "CONFEZIONATO",
        BatchStatus.ParzialmenteVenduto => "PARZIALMENTE_VENDUTO",
        BatchStatus.Esaurito => "ESAURITO",
        BatchStatus.Bloccato => "BLOCCATO",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public static string ToDbValue(this InventoryMovementType movementType) => movementType switch
    {
        InventoryMovementType.Load => "LOAD",
        InventoryMovementType.Unload => "UNLOAD",
        InventoryMovementType.ReturnIn => "RETURN_IN",
        InventoryMovementType.ReturnOut => "RETURN_OUT",
        InventoryMovementType.Waste => "WASTE",
        InventoryMovementType.TransferOut => "TRANSFER_OUT",
        InventoryMovementType.AdjustmentIn => "ADJUSTMENT_IN",
        InventoryMovementType.AdjustmentOut => "ADJUSTMENT_OUT",
        _ => throw new ArgumentOutOfRangeException(nameof(movementType), movementType, null)
    };

    public static string ToDbValue(this StockReservationStatus status) => status switch
    {
        StockReservationStatus.Active => "ACTIVE",
        StockReservationStatus.Reserved => "RESERVED",
        StockReservationStatus.Pending => "PENDING",
        StockReservationStatus.Released => "RELEASED",
        StockReservationStatus.Consumed => "CONSUMED",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public static string ToDbValue(this StockReservationType type) => type switch
    {
        StockReservationType.Order => "ORDER",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public static string ToDbValue(this OrderStatus status) => status switch
    {
        OrderStatus.Confermato => "CONFERMATO",
        OrderStatus.Spedito => "SPEDITO",
        OrderStatus.Evaso => "EVASO",
        OrderStatus.Confirmed => "CONFIRMED",
        OrderStatus.Shipped => "SHIPPED",
        OrderStatus.Completed => "COMPLETED",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public static string ToDbValue(this CertificationStatus status) => status switch
    {
        CertificationStatus.Active => "ACTIVE",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public static string ToDbValue(this MovementReferenceType type) => type switch
    {
        MovementReferenceType.GoodsReceipt => "GOODS_RECEIPT",
        MovementReferenceType.Order => "ORDER",
        MovementReferenceType.Processing => "PROCESSING",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public static string ToDbValue(this InvoiceDocumentType documentType) => documentType switch
    {
        InvoiceDocumentType.Fattura => "FATTURA",
        InvoiceDocumentType.Ddt => "DDT",
        _ => throw new ArgumentOutOfRangeException(nameof(documentType), documentType, null)
    };

    public static bool IsEligibleForSale(this BatchStatus status)
    {
        return status is BatchStatus.Ricevuto or BatchStatus.Disponibile or BatchStatus.ParzialmenteVenduto or BatchStatus.Confezionato;
    }

    public static bool IsStockExit(this OrderStatus status)
    {
        return status is OrderStatus.Spedito or OrderStatus.Shipped;
    }

    public static bool IsNegative(this InventoryMovementType movementType)
    {
        return movementType is InventoryMovementType.Unload or InventoryMovementType.ReturnOut or InventoryMovementType.Waste or InventoryMovementType.TransferOut or InventoryMovementType.AdjustmentOut;
    }

    public static bool CountsAsReserved(this StockReservationStatus status)
    {
        return status is StockReservationStatus.Active or StockReservationStatus.Reserved or StockReservationStatus.Pending;
    }

    public static bool TryParseBatchStatus(string value, out BatchStatus status)
    {
        return BatchStatuses.TryGetValue(value, out status);
    }

    public static bool TryParseInventoryMovementType(string value, out InventoryMovementType movementType)
    {
        return InventoryMovementTypes.TryGetValue(value, out movementType);
    }

    public static bool TryParseStockReservationStatus(string value, out StockReservationStatus status)
    {
        return StockReservationStatuses.TryGetValue(value, out status);
    }

    public static bool TryParseOrderStatus(string value, out OrderStatus status)
    {
        return OrderStatuses.TryGetValue(value, out status);
    }

    public static bool TryParseInvoiceDocumentType(string value, out InvoiceDocumentType documentType)
    {
        if (string.Equals(value, "FATTURA", StringComparison.OrdinalIgnoreCase))
        {
            documentType = InvoiceDocumentType.Fattura;
            return true;
        }

        if (string.Equals(value, "DDT", StringComparison.OrdinalIgnoreCase))
        {
            documentType = InvoiceDocumentType.Ddt;
            return true;
        }

        documentType = default;
        return false;
    }
}
