IF OBJECT_ID('dbo.usp_Orders_Overview', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.usp_Orders_Overview;
END
GO

CREATE PROCEDURE dbo.usp_Orders_Overview
    @OrderType NVARCHAR(20) = NULL,
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NormalizedOrderType NVARCHAR(20) = NULLIF(LTRIM(RTRIM(@OrderType)), '');
    DECLARE @NormalizedSearch NVARCHAR(100) = NULLIF(LTRIM(RTRIM(@Search)), '');

    ;WITH OrderLots AS
    (
        SELECT
            orderItem.OrderId,
            STRING_AGG(batch.BatchCode, ', ') AS LotCodes
        FROM dbo.OrderItems AS orderItem
        LEFT JOIN dbo.Batches AS batch
            ON batch.Id = orderItem.ReservedBatchId
        GROUP BY orderItem.OrderId
    ),
    OrderItemsCount AS
    (
        SELECT
            OrderId,
            COUNT(*) AS ItemCount
        FROM dbo.OrderItems
        GROUP BY OrderId
    )
    SELECT
        [order].Id AS OrderId,
        CONCAT('ORD-', RIGHT(CONCAT('0000', [order].Id), 4)) AS OrderCode,
        [order].CreatedAt,
        customer.Id AS CustomerId,
        customer.Name AS CustomerName,
        customer.Type AS CustomerType,
        [order].OrderType,
        [order].Status,
        [order].PaymentStatus,
        lots.LotCodes,
        [order].TotalAmount,
        [order].Currency,
        ISNULL(itemCount.ItemCount, 0) AS ItemCount,
        invoice.DocumentNumber AS InvoiceDocumentNumber,
        invoice.DocumentType AS InvoiceDocumentType
    FROM dbo.Orders AS [order]
    INNER JOIN dbo.Customers AS customer
        ON customer.Id = [order].CustomerId
    LEFT JOIN OrderLots AS lots
        ON lots.OrderId = [order].Id
    LEFT JOIN OrderItemsCount AS itemCount
        ON itemCount.OrderId = [order].Id
    LEFT JOIN dbo.Invoices AS invoice
        ON invoice.OrderId = [order].Id
    WHERE (@NormalizedOrderType IS NULL OR [order].OrderType = @NormalizedOrderType)
      AND (
          @NormalizedSearch IS NULL
          OR CAST([order].Id AS NVARCHAR(20)) LIKE '%' + @NormalizedSearch + '%'
          OR customer.Name LIKE '%' + @NormalizedSearch + '%'
          OR ISNULL(lots.LotCodes, N'') LIKE '%' + @NormalizedSearch + '%'
          OR ISNULL(invoice.DocumentNumber, N'') LIKE '%' + @NormalizedSearch + '%'
      )
    ORDER BY [order].CreatedAt DESC, [order].Id DESC;
END
GO
