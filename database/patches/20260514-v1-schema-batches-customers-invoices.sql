USE [StellaFruttaDb];
GO

IF COL_LENGTH('dbo.Batches', 'Variety') IS NULL
BEGIN
    ALTER TABLE dbo.Batches
    ADD Variety NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('dbo.Batches', 'InitialQuantity') IS NULL
BEGIN
    ALTER TABLE dbo.Batches
    ADD InitialQuantity DECIMAL(18, 3) NULL;
END
GO

IF COL_LENGTH('dbo.Batches', 'UnitOfMeasure') IS NULL
BEGIN
    ALTER TABLE dbo.Batches
    ADD UnitOfMeasure NVARCHAR(20) NULL;
END
GO

IF COL_LENGTH('dbo.Batches', 'SupplierDocumentId') IS NULL
BEGIN
    ALTER TABLE dbo.Batches
    ADD SupplierDocumentId INT NULL;
END
GO

IF COL_LENGTH('dbo.Batches', 'CertificationId') IS NULL
BEGIN
    ALTER TABLE dbo.Batches
    ADD CertificationId INT NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Batches_SupplierDocumentId' AND object_id = OBJECT_ID('dbo.Batches'))
BEGIN
    CREATE INDEX IX_Batches_SupplierDocumentId ON dbo.Batches (SupplierDocumentId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Batches_CertificationId' AND object_id = OBJECT_ID('dbo.Batches'))
BEGIN
    CREATE INDEX IX_Batches_CertificationId ON dbo.Batches (CertificationId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Batches_SupplierDocuments'
)
BEGIN
    ALTER TABLE dbo.Batches
    ADD CONSTRAINT FK_Batches_SupplierDocuments
    FOREIGN KEY (SupplierDocumentId) REFERENCES dbo.SupplierDocuments(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Batches_Certifications'
)
BEGIN
    ALTER TABLE dbo.Batches
    ADD CONSTRAINT FK_Batches_Certifications
    FOREIGN KEY (CertificationId) REFERENCES dbo.Certifications(Id);
END
GO

IF COL_LENGTH('dbo.Customers', 'SdiCode') IS NULL
BEGIN
    ALTER TABLE dbo.Customers
    ADD SdiCode NVARCHAR(20) NULL;
END
GO

IF OBJECT_ID('dbo.Invoices', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Invoices
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Invoices PRIMARY KEY,
        OrderId INT NOT NULL,
        CustomerId INT NOT NULL,
        DocumentNumber NVARCHAR(100) NOT NULL,
        DocumentType NVARCHAR(30) NOT NULL,
        TotalAmount DECIMAL(18, 2) NOT NULL,
        Currency NVARCHAR(10) NOT NULL,
        IssueDate DATETIME2 NOT NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Invoices_CreatedAt DEFAULT (sysdatetime()),
        UpdatedAt DATETIME2 NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Invoices_DocumentNumber' AND object_id = OBJECT_ID('dbo.Invoices'))
BEGIN
    CREATE UNIQUE INDEX UQ_Invoices_DocumentNumber ON dbo.Invoices (DocumentNumber);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Invoices_OrderId' AND object_id = OBJECT_ID('dbo.Invoices'))
BEGIN
    CREATE INDEX IX_Invoices_OrderId ON dbo.Invoices (OrderId);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Invoices_CustomerId' AND object_id = OBJECT_ID('dbo.Invoices'))
BEGIN
    CREATE INDEX IX_Invoices_CustomerId ON dbo.Invoices (CustomerId);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Invoices_Orders'
)
BEGIN
    ALTER TABLE dbo.Invoices
    ADD CONSTRAINT FK_Invoices_Orders
    FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_Invoices_Customers'
)
BEGIN
    ALTER TABLE dbo.Invoices
    ADD CONSTRAINT FK_Invoices_Customers
    FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id);
END
GO
