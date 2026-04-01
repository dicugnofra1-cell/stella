SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

IF DB_ID(N'StellaFruttaDb') IS NULL
BEGIN
    CREATE DATABASE [StellaFruttaDb];
END
GO

USE [StellaFruttaDb];
GO

IF OBJECT_ID(N'dbo.AuditLog', N'U') IS NOT NULL DROP TABLE dbo.AuditLog;
IF OBJECT_ID(N'dbo.NonConformities', N'U') IS NOT NULL DROP TABLE dbo.NonConformities;
IF OBJECT_ID(N'dbo.QualityChecks', N'U') IS NOT NULL DROP TABLE dbo.QualityChecks;
IF OBJECT_ID(N'dbo.StockReservations', N'U') IS NOT NULL DROP TABLE dbo.StockReservations;
IF OBJECT_ID(N'dbo.OrderItems', N'U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID(N'dbo.Orders', N'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID(N'dbo.InventoryMovements', N'U') IS NOT NULL DROP TABLE dbo.InventoryMovements;
IF OBJECT_ID(N'dbo.PublicTraceViews', N'U') IS NOT NULL DROP TABLE dbo.PublicTraceViews;
IF OBJECT_ID(N'dbo.BatchLinks', N'U') IS NOT NULL DROP TABLE dbo.BatchLinks;
IF OBJECT_ID(N'dbo.Batches', N'U') IS NOT NULL DROP TABLE dbo.Batches;
IF OBJECT_ID(N'dbo.CustomerAddresses', N'U') IS NOT NULL DROP TABLE dbo.CustomerAddresses;
IF OBJECT_ID(N'dbo.Customers', N'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID(N'dbo.Certifications', N'U') IS NOT NULL DROP TABLE dbo.Certifications;
IF OBJECT_ID(N'dbo.SupplierDocuments', N'U') IS NOT NULL DROP TABLE dbo.SupplierDocuments;
IF OBJECT_ID(N'dbo.Suppliers', N'U') IS NOT NULL DROP TABLE dbo.Suppliers;
IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL DROP TABLE dbo.Products;
GO

CREATE TABLE dbo.Products
(
    Id int IDENTITY(1,1) NOT NULL,
    Sku nvarchar(50) NOT NULL,
    Name nvarchar(150) NOT NULL,
    Description nvarchar(max) NULL,
    UnitOfMeasure nvarchar(20) NOT NULL,
    Category nvarchar(100) NULL,
    ChannelB2BEnabled bit NOT NULL CONSTRAINT DF_Products_ChannelB2BEnabled DEFAULT (1),
    ChannelB2CEnabled bit NOT NULL CONSTRAINT DF_Products_ChannelB2CEnabled DEFAULT (1),
    Active bit NOT NULL CONSTRAINT DF_Products_Active DEFAULT (1),
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT (sysdatetime()),
    UpdatedAt datetime2 NULL,
    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Products_Sku UNIQUE (Sku)
);
GO

CREATE TABLE dbo.Suppliers
(
    Id int IDENTITY(1,1) NOT NULL,
    Name nvarchar(200) NOT NULL,
    VatNumber nvarchar(30) NULL,
    Address nvarchar(250) NULL,
    Email nvarchar(150) NULL,
    Phone nvarchar(50) NULL,
    Status nvarchar(30) NOT NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_Suppliers_CreatedAt DEFAULT (sysdatetime()),
    UpdatedAt datetime2 NULL,
    CONSTRAINT PK_Suppliers PRIMARY KEY CLUSTERED (Id)
);
GO

CREATE TABLE dbo.SupplierDocuments
(
    Id int IDENTITY(1,1) NOT NULL,
    SupplierId int NOT NULL,
    DocumentType nvarchar(50) NOT NULL,
    FileName nvarchar(255) NOT NULL,
    StoragePath nvarchar(500) NOT NULL,
    UploadedAt datetime2 NOT NULL CONSTRAINT DF_SupplierDocuments_UploadedAt DEFAULT (sysdatetime()),
    UploadedBy nvarchar(100) NULL,
    CONSTRAINT PK_SupplierDocuments PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_SupplierDocuments_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id)
);
GO

CREATE TABLE dbo.Certifications
(
    Id int IDENTITY(1,1) NOT NULL,
    SupplierId int NOT NULL,
    Type nvarchar(50) NOT NULL,
    Authority nvarchar(150) NOT NULL,
    Code nvarchar(100) NULL,
    IssueDate date NULL,
    ExpiryDate date NOT NULL,
    Status nvarchar(30) NOT NULL,
    DocumentId int NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_Certifications_CreatedAt DEFAULT (sysdatetime()),
    CONSTRAINT PK_Certifications PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Certifications_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id),
    CONSTRAINT FK_Certifications_SupplierDocuments FOREIGN KEY (DocumentId) REFERENCES dbo.SupplierDocuments(Id)
);
GO

CREATE TABLE dbo.Customers
(
    Id int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(20) NOT NULL,
    Name nvarchar(200) NOT NULL,
    VatNumber nvarchar(30) NULL,
    Email nvarchar(150) NOT NULL,
    Phone nvarchar(50) NULL,
    Status nvarchar(30) NOT NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT (sysdatetime()),
    UpdatedAt datetime2 NULL,
    CONSTRAINT PK_Customers PRIMARY KEY CLUSTERED (Id)
);
GO

CREATE TABLE dbo.CustomerAddresses
(
    Id int IDENTITY(1,1) NOT NULL,
    CustomerId int NOT NULL,
    AddressType nvarchar(30) NOT NULL,
    RecipientName nvarchar(200) NOT NULL,
    Street nvarchar(250) NOT NULL,
    Street2 nvarchar(250) NULL,
    City nvarchar(100) NOT NULL,
    PostalCode nvarchar(20) NOT NULL,
    Province nvarchar(50) NOT NULL,
    Country nvarchar(100) NOT NULL,
    IsDefault bit NOT NULL CONSTRAINT DF_CustomerAddresses_IsDefault DEFAULT (0),
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_CustomerAddresses_CreatedAt DEFAULT (sysdatetime()),
    UpdatedAt datetime2 NULL,
    CONSTRAINT PK_CustomerAddresses PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_CustomerAddresses_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id)
);
GO

CREATE TABLE dbo.Batches
(
    Id int IDENTITY(1,1) NOT NULL,
    BatchCode nvarchar(100) NOT NULL,
    ProductId int NOT NULL,
    BatchType nvarchar(20) NOT NULL,
    Status nvarchar(30) NOT NULL,
    BioFlag bit NOT NULL,
    SupplierId int NULL,
    ProductionDate date NULL,
    ExpirationDate date NULL,
    Notes nvarchar(max) NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_Batches_CreatedAt DEFAULT (sysdatetime()),
    UpdatedAt datetime2 NULL,
    CONSTRAINT PK_Batches PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_Batches_BatchCode UNIQUE (BatchCode),
    CONSTRAINT FK_Batches_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT FK_Batches_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id)
);
GO

CREATE TABLE dbo.BatchLinks
(
    Id int IDENTITY(1,1) NOT NULL,
    ParentBatchId int NOT NULL,
    ChildBatchId int NOT NULL,
    QuantityUsed decimal(18,3) NOT NULL,
    UnitOfMeasure nvarchar(20) NOT NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_BatchLinks_CreatedAt DEFAULT (sysdatetime()),
    CONSTRAINT PK_BatchLinks PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT CK_BatchLinks_NoSelfReference CHECK (ParentBatchId <> ChildBatchId),
    CONSTRAINT FK_BatchLinks_ParentBatch FOREIGN KEY (ParentBatchId) REFERENCES dbo.Batches(Id),
    CONSTRAINT FK_BatchLinks_ChildBatch FOREIGN KEY (ChildBatchId) REFERENCES dbo.Batches(Id)
);
GO

CREATE TABLE dbo.PublicTraceViews
(
    BatchId int NOT NULL,
    BatchCode nvarchar(100) NOT NULL,
    ProductName nvarchar(150) NOT NULL,
    BioFlag bit NOT NULL,
    OriginInfo nvarchar(max) NULL,
    MainDates nvarchar(max) NULL,
    LastUpdatedAt datetime2 NOT NULL CONSTRAINT DF_PublicTraceViews_LastUpdatedAt DEFAULT (sysdatetime()),
    CONSTRAINT PK_PublicTraceViews PRIMARY KEY CLUSTERED (BatchId),
    CONSTRAINT FK_PublicTraceViews_Batches FOREIGN KEY (BatchId) REFERENCES dbo.Batches(Id)
);
GO

CREATE TABLE dbo.InventoryMovements
(
    Id int IDENTITY(1,1) NOT NULL,
    ProductId int NOT NULL,
    BatchId int NOT NULL,
    MovementType nvarchar(30) NOT NULL,
    Quantity decimal(18,3) NOT NULL,
    MovementDate datetime2 NOT NULL CONSTRAINT DF_InventoryMovements_MovementDate DEFAULT (sysdatetime()),
    Reason nvarchar(250) NULL,
    ReferenceType nvarchar(50) NULL,
    ReferenceId nvarchar(100) NULL,
    UserId nvarchar(100) NOT NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_InventoryMovements_CreatedAt DEFAULT (sysdatetime()),
    CONSTRAINT PK_InventoryMovements PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_InventoryMovements_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT FK_InventoryMovements_Batches FOREIGN KEY (BatchId) REFERENCES dbo.Batches(Id)
);
GO

CREATE TABLE dbo.Orders
(
    Id int IDENTITY(1,1) NOT NULL,
    CustomerId int NOT NULL,
    OrderType nvarchar(20) NOT NULL,
    Status nvarchar(30) NOT NULL,
    PaymentStatus nvarchar(30) NULL,
    TotalAmount decimal(18,2) NOT NULL,
    Currency nvarchar(10) NOT NULL,
    Notes nvarchar(max) NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT (sysdatetime()),
    UpdatedAt datetime2 NULL,
    CONSTRAINT PK_Orders PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(Id)
);
GO

CREATE TABLE dbo.OrderItems
(
    Id int IDENTITY(1,1) NOT NULL,
    OrderId int NOT NULL,
    ProductId int NOT NULL,
    Quantity decimal(18,3) NOT NULL,
    UnitPrice decimal(18,2) NOT NULL,
    TaxAmount decimal(18,2) NULL,
    ReservedBatchId int NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_OrderItems_CreatedAt DEFAULT (sysdatetime()),
    CONSTRAINT PK_OrderItems PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT FK_OrderItems_Batches FOREIGN KEY (ReservedBatchId) REFERENCES dbo.Batches(Id)
);
GO

CREATE TABLE dbo.StockReservations
(
    Id int IDENTITY(1,1) NOT NULL,
    OrderId int NOT NULL,
    OrderItemId int NOT NULL,
    ProductId int NOT NULL,
    BatchId int NULL,
    Quantity decimal(18,3) NOT NULL,
    Status nvarchar(30) NOT NULL,
    ReservationType nvarchar(30) NOT NULL,
    ExpiresAt datetime2 NULL,
    CreatedAt datetime2 NOT NULL CONSTRAINT DF_StockReservations_CreatedAt DEFAULT (sysdatetime()),
    UpdatedAt datetime2 NULL,
    ReleasedAt datetime2 NULL,
    ConsumedAt datetime2 NULL,
    Notes nvarchar(max) NULL,
    CONSTRAINT PK_StockReservations PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_StockReservations_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
    CONSTRAINT FK_StockReservations_OrderItems FOREIGN KEY (OrderItemId) REFERENCES dbo.OrderItems(Id),
    CONSTRAINT FK_StockReservations_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT FK_StockReservations_Batches FOREIGN KEY (BatchId) REFERENCES dbo.Batches(Id)
);
GO

CREATE TABLE dbo.QualityChecks
(
    Id int IDENTITY(1,1) NOT NULL,
    BatchId int NOT NULL,
    ChecklistVersion nvarchar(50) NOT NULL,
    Result nvarchar(20) NOT NULL,
    Notes nvarchar(max) NULL,
    CheckedAt datetime2 NOT NULL CONSTRAINT DF_QualityChecks_CheckedAt DEFAULT (sysdatetime()),
    CheckedBy nvarchar(100) NOT NULL,
    CONSTRAINT PK_QualityChecks PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_QualityChecks_Batches FOREIGN KEY (BatchId) REFERENCES dbo.Batches(Id)
);
GO

CREATE TABLE dbo.NonConformities
(
    Id int IDENTITY(1,1) NOT NULL,
    BatchId int NOT NULL,
    Severity nvarchar(20) NOT NULL,
    Status nvarchar(30) NOT NULL,
    Description nvarchar(max) NOT NULL,
    CorrectiveAction nvarchar(max) NULL,
    OpenedBy nvarchar(100) NOT NULL,
    OpenedAt datetime2 NOT NULL CONSTRAINT DF_NonConformities_OpenedAt DEFAULT (sysdatetime()),
    ClosedBy nvarchar(100) NULL,
    ClosedAt datetime2 NULL,
    CONSTRAINT PK_NonConformities PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_NonConformities_Batches FOREIGN KEY (BatchId) REFERENCES dbo.Batches(Id)
);
GO

CREATE TABLE dbo.AuditLog
(
    Id bigint IDENTITY(1,1) NOT NULL,
    EntityName nvarchar(100) NOT NULL,
    EntityId nvarchar(100) NOT NULL,
    Action nvarchar(50) NOT NULL,
    OldValues nvarchar(max) NULL,
    NewValues nvarchar(max) NULL,
    ChangedBy nvarchar(100) NOT NULL,
    ChangedAt datetime2 NOT NULL CONSTRAINT DF_AuditLog_ChangedAt DEFAULT (sysdatetime()),
    CorrelationId nvarchar(100) NULL,
    CONSTRAINT PK_AuditLog PRIMARY KEY CLUSTERED (Id)
);
GO

CREATE INDEX IX_CustomerAddresses_CustomerId_AddressType ON dbo.CustomerAddresses (CustomerId, AddressType);
GO
CREATE UNIQUE INDEX UX_CustomerAddresses_DefaultByType ON dbo.CustomerAddresses (CustomerId, AddressType) WHERE IsDefault = 1;
GO
CREATE INDEX IX_Batches_ProductId_Status ON dbo.Batches (ProductId, Status);
GO
CREATE INDEX IX_BatchLinks_ParentBatchId ON dbo.BatchLinks (ParentBatchId);
GO
CREATE INDEX IX_BatchLinks_ChildBatchId ON dbo.BatchLinks (ChildBatchId);
GO
CREATE INDEX IX_InventoryMovements_BatchId_MovementDate ON dbo.InventoryMovements (BatchId, MovementDate);
GO
CREATE INDEX IX_InventoryMovements_ProductId_MovementDate ON dbo.InventoryMovements (ProductId, MovementDate);
GO
CREATE INDEX IX_Orders_CustomerId_CreatedAt ON dbo.Orders (CustomerId, CreatedAt);
GO
CREATE INDEX IX_Orders_Status ON dbo.Orders (Status);
GO
CREATE INDEX IX_StockReservations_OrderId ON dbo.StockReservations (OrderId);
GO
CREATE INDEX IX_StockReservations_OrderItemId ON dbo.StockReservations (OrderItemId);
GO
CREATE INDEX IX_StockReservations_ProductId_Status ON dbo.StockReservations (ProductId, Status);
GO
CREATE INDEX IX_StockReservations_BatchId_Status ON dbo.StockReservations (BatchId, Status);
GO
CREATE INDEX IX_AuditLog_EntityName_EntityId ON dbo.AuditLog (EntityName, EntityId);
GO
CREATE INDEX IX_AuditLog_ChangedAt ON dbo.AuditLog (ChangedAt);
GO
