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
