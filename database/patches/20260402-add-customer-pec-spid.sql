USE [StellaFruttaDb];
GO

IF COL_LENGTH('dbo.Customers', 'Pec') IS NULL
BEGIN
    ALTER TABLE dbo.Customers
    ADD Pec NVARCHAR(150) NULL;
END
GO

IF COL_LENGTH('dbo.Customers', 'SpidIdentifier') IS NULL
BEGIN
    ALTER TABLE dbo.Customers
    ADD SpidIdentifier NVARCHAR(100) NULL;
END
GO
