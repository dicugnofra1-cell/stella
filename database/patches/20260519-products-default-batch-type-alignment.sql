IF COL_LENGTH('dbo.Products', 'DefaultBatchType') IS NULL
BEGIN
    ALTER TABLE dbo.Products
    ADD DefaultBatchType NVARCHAR(30) NULL;
END
GO

UPDATE p
SET p.DefaultBatchType =
    CASE
        WHEN UPPER(ISNULL(p.Name, '')) LIKE '%PELATA%' THEN 'MANDORLA_PELATA'
        WHEN UPPER(ISNULL(p.Name, '')) LIKE '%NATURALE%' THEN 'MANDORLA_NATURALE'
        WHEN UPPER(ISNULL(p.Name, '')) LIKE '%GUSCIO%' THEN 'MANDORLA_CON_GUSCIO'
        WHEN UPPER(ISNULL(p.Name, '')) LIKE '%GRANELLA%' THEN 'GRANELLA'
        WHEN UPPER(ISNULL(p.Name, '')) LIKE '%FARINA%' THEN 'FARINA'
        ELSE COALESCE(freq.BatchType, 'MANDORLA_NATURALE')
    END
FROM dbo.Products p
OUTER APPLY
(
    SELECT TOP (1) b.BatchType
    FROM dbo.Batches b
    WHERE b.ProductId = p.Id
      AND b.BatchType IS NOT NULL
    GROUP BY b.BatchType
    ORDER BY COUNT(*) DESC, b.BatchType ASC
) freq
WHERE p.DefaultBatchType IS NULL
   OR LTRIM(RTRIM(p.DefaultBatchType)) = '';
GO

UPDATE dbo.Products
SET DefaultBatchType = UPPER(LTRIM(RTRIM(DefaultBatchType)))
WHERE DefaultBatchType IS NOT NULL;
GO

UPDATE b
SET b.BatchType = p.DefaultBatchType
FROM dbo.Batches b
INNER JOIN dbo.Products p ON p.Id = b.ProductId
WHERE b.BatchCode LIKE 'LOTTO-%'
  AND p.DefaultBatchType IS NOT NULL
  AND UPPER(LTRIM(RTRIM(ISNULL(b.BatchType, '')))) <> p.DefaultBatchType;
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Products')
      AND name = 'DefaultBatchType'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE dbo.Products
    ALTER COLUMN DefaultBatchType NVARCHAR(30) NOT NULL;
END
GO
