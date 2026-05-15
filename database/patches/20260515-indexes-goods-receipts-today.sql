IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_Batches_CreatedAt_MerceToday'
      AND object_id = OBJECT_ID('dbo.Batches')
)
BEGIN
    CREATE INDEX IX_Batches_CreatedAt_MerceToday
        ON dbo.Batches (CreatedAt, Id)
        INCLUDE (
            ProductId,
            SupplierId,
            BatchCode,
            BatchType,
            BioFlag,
            InitialQuantity,
            UnitOfMeasure,
            Status,
            Variety,
            Notes,
            CertificationId
        );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_InventoryMovements_Load_ByBatch'
      AND object_id = OBJECT_ID('dbo.InventoryMovements')
)
BEGIN
    CREATE INDEX IX_InventoryMovements_Load_ByBatch
        ON dbo.InventoryMovements (BatchId, MovementDate, Id)
        INCLUDE (UserId)
        WHERE MovementType = 'LOAD';
END
GO
