IF COL_LENGTH('dbo.Batches', 'PurchaseUnitPrice') IS NULL
BEGIN
    ALTER TABLE dbo.Batches
    ADD PurchaseUnitPrice DECIMAL(18, 2) NULL;
END
GO

IF OBJECT_ID('dbo.usp_GoodsReceipts_Today', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_GoodsReceipts_Today;
END
GO

CREATE PROCEDURE dbo.usp_GoodsReceipts_Today
    @Day DATE,
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @From DATETIME2(7) = CAST(@Day AS DATETIME2(7));
    DECLARE @To DATETIME2(7) = DATEADD(DAY, 1, @From);
    DECLARE @NormalizedSearch NVARCHAR(100) = NULLIF(LTRIM(RTRIM(@Search)), '');

    ;WITH LoadMovements AS
    (
        SELECT
            movement.BatchId,
            movement.UserId,
            ROW_NUMBER() OVER (
                PARTITION BY movement.BatchId
                ORDER BY movement.MovementDate ASC, movement.Id ASC
            ) AS RowNumber
        FROM dbo.InventoryMovements AS movement
        WHERE movement.MovementType = 'LOAD'
    )
    SELECT
        batch.Id AS BatchId,
        batch.ProductId,
        batch.BatchCode,
        batch.CreatedAt,
        ISNULL(supplier.Name, N'Fornitore non collegato') AS SupplierName,
        product.Name AS ProductName,
        batch.BatchType,
        batch.BioFlag,
        ISNULL(batch.InitialQuantity, CONVERT(DECIMAL(18, 3), 0)) AS Quantity,
        batch.PurchaseUnitPrice,
        ISNULL(batch.UnitOfMeasure, product.UnitOfMeasure) AS UnitOfMeasure,
        batch.Status,
        batch.Variety,
        batch.Notes,
        ISNULL(loadMovement.UserId, N'Sistema') AS [Operator],
        batch.SupplierId,
        batch.CertificationId
    FROM dbo.Batches AS batch
    INNER JOIN dbo.Products AS product
        ON product.Id = batch.ProductId
    LEFT JOIN dbo.Suppliers AS supplier
        ON supplier.Id = batch.SupplierId
    LEFT JOIN LoadMovements AS loadMovement
        ON loadMovement.BatchId = batch.Id
       AND loadMovement.RowNumber = 1
    WHERE batch.CreatedAt >= @From
      AND batch.CreatedAt < @To
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

IF OBJECT_ID('dbo.usp_GoodsReceipts_ActiveHistory', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_GoodsReceipts_ActiveHistory;
END
GO

CREATE PROCEDURE dbo.usp_GoodsReceipts_ActiveHistory
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedSearch NVARCHAR(100) = NULLIF(LTRIM(RTRIM(@Search)), '');

    ;WITH LoadMovements AS
    (
        SELECT
            movement.BatchId,
            movement.UserId,
            ROW_NUMBER() OVER (
                PARTITION BY movement.BatchId
                ORDER BY movement.MovementDate ASC, movement.Id ASC
            ) AS RowNumber
        FROM dbo.InventoryMovements AS movement
        WHERE movement.MovementType = 'LOAD'
    ),
    PhysicalBalances AS
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
    ),
    ActiveBatches AS
    (
        SELECT
            batch.Id AS BatchId
        FROM dbo.Batches AS batch
        LEFT JOIN PhysicalBalances AS physical
            ON physical.BatchId = batch.Id
        LEFT JOIN ReservedBalances AS reserved
            ON reserved.BatchId = batch.Id
        WHERE ISNULL(physical.PhysicalStock, CONVERT(DECIMAL(18, 3), 0)) - ISNULL(reserved.ReservedStock, CONVERT(DECIMAL(18, 3), 0)) > 0
    ),
    RelevantLineage AS
    (
        SELECT activeBatch.BatchId
        FROM ActiveBatches AS activeBatch

        UNION ALL

        SELECT batchLink.ParentBatchId
        FROM dbo.BatchLinks AS batchLink
        INNER JOIN RelevantLineage AS lineage
            ON lineage.BatchId = batchLink.ChildBatchId
    ),
    RelevantBatches AS
    (
        SELECT DISTINCT BatchId
        FROM RelevantLineage
    )
    SELECT
        batch.Id AS BatchId,
        batch.ProductId,
        batch.BatchCode,
        batch.CreatedAt,
        ISNULL(supplier.Name, N'Fornitore non collegato') AS SupplierName,
        product.Name AS ProductName,
        batch.BatchType,
        batch.BioFlag,
        ISNULL(batch.InitialQuantity, CONVERT(DECIMAL(18, 3), 0)) AS Quantity,
        batch.PurchaseUnitPrice,
        ISNULL(batch.UnitOfMeasure, product.UnitOfMeasure) AS UnitOfMeasure,
        batch.Status,
        batch.Variety,
        batch.Notes,
        ISNULL(loadMovement.UserId, N'Sistema') AS [Operator],
        batch.SupplierId,
        batch.CertificationId
    FROM dbo.Batches AS batch
    INNER JOIN dbo.Products AS product
        ON product.Id = batch.ProductId
    LEFT JOIN dbo.Suppliers AS supplier
        ON supplier.Id = batch.SupplierId
    LEFT JOIN LoadMovements AS loadMovement
        ON loadMovement.BatchId = batch.Id
       AND loadMovement.RowNumber = 1
    LEFT JOIN PhysicalBalances AS physical
        ON physical.BatchId = batch.Id
    LEFT JOIN ReservedBalances AS reserved
        ON reserved.BatchId = batch.Id
    INNER JOIN RelevantBatches AS relevant
        ON relevant.BatchId = batch.Id
    WHERE
      (
          ISNULL(physical.PhysicalStock, CONVERT(DECIMAL(18, 3), 0)) - ISNULL(reserved.ReservedStock, CONVERT(DECIMAL(18, 3), 0)) > 0
          OR EXISTS
          (
              SELECT 1
              FROM dbo.BatchLinks AS batchLink
              WHERE batchLink.ParentBatchId = batch.Id
          )
      )
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
