IF OBJECT_ID('dbo.usp_Batches_Active', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_Batches_Active;
END
GO

CREATE PROCEDURE dbo.usp_Batches_Active
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedSearch NVARCHAR(100) = NULLIF(LTRIM(RTRIM(@Search)), '');

    ;WITH PhysicalBalances AS
    (
        SELECT
            movement.BatchId,
            SUM(
                CASE
                    WHEN movement.MovementType IN ('UNLOAD', 'RETURN_OUT', 'WASTE', 'TRANSFER_OUT', 'ADJUSTMENT_OUT')
                        THEN -movement.Quantity
                    ELSE movement.Quantity
                END
            ) AS PhysicalStock
        FROM dbo.InventoryMovements AS movement
        GROUP BY movement.BatchId
    ),
    ReservedBalances AS
    (
        SELECT
            reservation.BatchId,
            SUM(reservation.Quantity) AS ReservedStock
        FROM dbo.StockReservations AS reservation
        WHERE reservation.BatchId IS NOT NULL
          AND reservation.Status IN ('ACTIVE', 'RESERVED', 'PENDING')
        GROUP BY reservation.BatchId
    )
    SELECT
        batch.Id AS BatchId,
        batch.BatchCode,
        batch.ProductId,
        product.Name AS ProductName,
        batch.BatchType,
        batch.Status,
        batch.BioFlag,
        batch.Variety,
        ISNULL(physical.PhysicalStock, CONVERT(DECIMAL(18, 3), 0)) AS PhysicalStock,
        ISNULL(reserved.ReservedStock, CONVERT(DECIMAL(18, 3), 0)) AS ReservedStock,
        ISNULL(physical.PhysicalStock, CONVERT(DECIMAL(18, 3), 0)) - ISNULL(reserved.ReservedStock, CONVERT(DECIMAL(18, 3), 0)) AS AvailableStock,
        ISNULL(batch.UnitOfMeasure, product.UnitOfMeasure) AS UnitOfMeasure,
        ISNULL(supplier.Name, N'Fornitore non collegato') AS SupplierName,
        batch.SupplierId,
        CAST(CASE WHEN trace.BatchId IS NULL THEN 0 ELSE 1 END AS BIT) AS QrActive,
        batch.CreatedAt
    FROM dbo.Batches AS batch
    INNER JOIN dbo.Products AS product
        ON product.Id = batch.ProductId
    LEFT JOIN dbo.Suppliers AS supplier
        ON supplier.Id = batch.SupplierId
    LEFT JOIN PhysicalBalances AS physical
        ON physical.BatchId = batch.Id
    LEFT JOIN ReservedBalances AS reserved
        ON reserved.BatchId = batch.Id
    LEFT JOIN dbo.PublicTraceViews AS trace
        ON trace.BatchId = batch.Id
    WHERE ISNULL(physical.PhysicalStock, CONVERT(DECIMAL(18, 3), 0)) - ISNULL(reserved.ReservedStock, CONVERT(DECIMAL(18, 3), 0)) > 0
      AND (
          @NormalizedSearch IS NULL
          OR batch.BatchCode LIKE '%' + @NormalizedSearch + '%'
          OR product.Name LIKE '%' + @NormalizedSearch + '%'
          OR ISNULL(supplier.Name, N'') LIKE '%' + @NormalizedSearch + '%'
          OR ISNULL(batch.Variety, N'') LIKE '%' + @NormalizedSearch + '%'
      )
    ORDER BY batch.CreatedAt DESC, batch.Id DESC;
END
GO
