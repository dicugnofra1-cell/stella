IF COL_LENGTH('dbo.Invoices', 'Source') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices
    ADD Source NVARCHAR(30) NULL;
END
GO

IF COL_LENGTH('dbo.Invoices', 'SyncStatus') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices
    ADD SyncStatus NVARCHAR(30) NULL;
END
GO

IF COL_LENGTH('dbo.Invoices', 'ExternalProvider') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices
    ADD ExternalProvider NVARCHAR(50) NULL;
END
GO

IF COL_LENGTH('dbo.Invoices', 'ExternalDocumentId') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices
    ADD ExternalDocumentId NVARCHAR(150) NULL;
END
GO

IF COL_LENGTH('dbo.Invoices', 'ExternalDocumentNumber') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices
    ADD ExternalDocumentNumber NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('dbo.Invoices', 'ExternalSyncAt') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices
    ADD ExternalSyncAt DATETIME2(7) NULL;
END
GO

IF COL_LENGTH('dbo.Invoices', 'ExternalSyncError') IS NULL
BEGIN
    ALTER TABLE dbo.Invoices
    ADD ExternalSyncError NVARCHAR(1000) NULL;
END
GO

UPDATE dbo.Invoices
SET
    Source = ISNULL(Source, N'STELLA'),
    SyncStatus = ISNULL(SyncStatus, N'PREPARATO')
WHERE Source IS NULL
   OR SyncStatus IS NULL;
GO

ALTER TABLE dbo.Invoices
ALTER COLUMN Source NVARCHAR(30) NOT NULL;
GO

ALTER TABLE dbo.Invoices
ALTER COLUMN SyncStatus NVARCHAR(30) NOT NULL;
GO
