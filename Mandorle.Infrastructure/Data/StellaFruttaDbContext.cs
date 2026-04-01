using System;
using System.Collections.Generic;
using Mandorle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Data;

public partial class StellaFruttaDbContext : DbContext
{
    public StellaFruttaDbContext(DbContextOptions<StellaFruttaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<BatchLink> BatchLinks { get; set; }

    public virtual DbSet<Certification> Certifications { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }

    public virtual DbSet<InventoryMovement> InventoryMovements { get; set; }

    public virtual DbSet<NonConformity> NonConformities { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<PublicTraceView> PublicTraceViews { get; set; }

    public virtual DbSet<QualityCheck> QualityChecks { get; set; }

    public virtual DbSet<StockReservation> StockReservations { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplierDocument> SupplierDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLog");

            entity.HasIndex(e => e.ChangedAt, "IX_AuditLog_ChangedAt");

            entity.HasIndex(e => new { e.EntityName, e.EntityId }, "IX_AuditLog_EntityName_EntityId");

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(sysdatetime())", "DF_AuditLog_ChangedAt");
            entity.Property(e => e.ChangedBy).HasMaxLength(100);
            entity.Property(e => e.CorrelationId).HasMaxLength(100);
            entity.Property(e => e.EntityId).HasMaxLength(100);
            entity.Property(e => e.EntityName).HasMaxLength(100);
        });

        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasIndex(e => new { e.ProductId, e.Status }, "IX_Batches_ProductId_Status");

            entity.HasIndex(e => e.BatchCode, "UQ_Batches_BatchCode").IsUnique();

            entity.Property(e => e.BatchCode).HasMaxLength(100);
            entity.Property(e => e.BatchType).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Batches_CreatedAt");
            entity.Property(e => e.Status).HasMaxLength(30);

            entity.HasOne(d => d.Product).WithMany(p => p.Batches)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Batches_Products");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Batches)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_Batches_Suppliers");
        });

        modelBuilder.Entity<BatchLink>(entity =>
        {
            entity.HasIndex(e => e.ChildBatchId, "IX_BatchLinks_ChildBatchId");

            entity.HasIndex(e => e.ParentBatchId, "IX_BatchLinks_ParentBatchId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_BatchLinks_CreatedAt");
            entity.Property(e => e.QuantityUsed).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);

            entity.HasOne(d => d.ChildBatch).WithMany(p => p.BatchLinkChildBatches)
                .HasForeignKey(d => d.ChildBatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BatchLinks_ChildBatch");

            entity.HasOne(d => d.ParentBatch).WithMany(p => p.BatchLinkParentBatches)
                .HasForeignKey(d => d.ParentBatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BatchLinks_ParentBatch");
        });

        modelBuilder.Entity<Certification>(entity =>
        {
            entity.Property(e => e.Authority).HasMaxLength(150);
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Certifications_CreatedAt");
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Document).WithMany(p => p.Certifications)
                .HasForeignKey(d => d.DocumentId)
                .HasConstraintName("FK_Certifications_SupplierDocuments");

            entity.HasOne(d => d.Supplier).WithMany(p => p.Certifications)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certifications_Suppliers");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Customers_CreatedAt");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.Type).HasMaxLength(20);
            entity.Property(e => e.VatNumber).HasMaxLength(30);
        });

        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.HasIndex(e => new { e.CustomerId, e.AddressType }, "IX_CustomerAddresses_CustomerId_AddressType");

            entity.HasIndex(e => new { e.CustomerId, e.AddressType }, "UX_CustomerAddresses_DefaultByType")
                .IsUnique()
                .HasFilter("([IsDefault]=(1))");

            entity.Property(e => e.AddressType).HasMaxLength(30);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_CustomerAddresses_CreatedAt");
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Province).HasMaxLength(50);
            entity.Property(e => e.RecipientName).HasMaxLength(200);
            entity.Property(e => e.Street).HasMaxLength(250);
            entity.Property(e => e.Street2).HasMaxLength(250);

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerAddresses)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAddresses_Customers");
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasIndex(e => new { e.BatchId, e.MovementDate }, "IX_InventoryMovements_BatchId_MovementDate");

            entity.HasIndex(e => new { e.ProductId, e.MovementDate }, "IX_InventoryMovements_ProductId_MovementDate");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_InventoryMovements_CreatedAt");
            entity.Property(e => e.MovementDate).HasDefaultValueSql("(sysdatetime())", "DF_InventoryMovements_MovementDate");
            entity.Property(e => e.MovementType).HasMaxLength(30);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.Reason).HasMaxLength(250);
            entity.Property(e => e.ReferenceId).HasMaxLength(100);
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.UserId).HasMaxLength(100);

            entity.HasOne(d => d.Batch).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryMovements_Batches");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryMovements_Products");
        });

        modelBuilder.Entity<NonConformity>(entity =>
        {
            entity.Property(e => e.ClosedBy).HasMaxLength(100);
            entity.Property(e => e.OpenedAt).HasDefaultValueSql("(sysdatetime())", "DF_NonConformities_OpenedAt");
            entity.Property(e => e.OpenedBy).HasMaxLength(100);
            entity.Property(e => e.Severity).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(30);

            entity.HasOne(d => d.Batch).WithMany(p => p.NonConformities)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NonConformities_Batches");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => new { e.CustomerId, e.CreatedAt }, "IX_Orders_CustomerId_CreatedAt");

            entity.HasIndex(e => e.Status, "IX_Orders_Status");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Orders_CreatedAt");
            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.OrderType).HasMaxLength(20);
            entity.Property(e => e.PaymentStatus).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_OrderItems_CreatedAt");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Products");

            entity.HasOne(d => d.ReservedBatch).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ReservedBatchId)
                .HasConstraintName("FK_OrderItems_Batches");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Sku, "UQ_Products_Sku").IsUnique();

            entity.Property(e => e.Active).HasDefaultValue(true, "DF_Products_Active");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ChannelB2BEnabled)
                .HasDefaultValue(true, "DF_Products_ChannelB2BEnabled")
                .HasColumnName("ChannelB2BEnabled");
            entity.Property(e => e.ChannelB2CEnabled)
                .HasDefaultValue(true, "DF_Products_ChannelB2CEnabled")
                .HasColumnName("ChannelB2CEnabled");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Products_CreatedAt");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Sku).HasMaxLength(50);
            entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);
        });

        modelBuilder.Entity<PublicTraceView>(entity =>
        {
            entity.HasKey(e => e.BatchId);

            entity.Property(e => e.BatchId).ValueGeneratedNever();
            entity.Property(e => e.BatchCode).HasMaxLength(100);
            entity.Property(e => e.LastUpdatedAt).HasDefaultValueSql("(sysdatetime())", "DF_PublicTraceViews_LastUpdatedAt");
            entity.Property(e => e.ProductName).HasMaxLength(150);

            entity.HasOne(d => d.Batch).WithOne(p => p.PublicTraceView)
                .HasForeignKey<PublicTraceView>(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublicTraceViews_Batches");
        });

        modelBuilder.Entity<QualityCheck>(entity =>
        {
            entity.Property(e => e.CheckedAt).HasDefaultValueSql("(sysdatetime())", "DF_QualityChecks_CheckedAt");
            entity.Property(e => e.CheckedBy).HasMaxLength(100);
            entity.Property(e => e.ChecklistVersion).HasMaxLength(50);
            entity.Property(e => e.Result).HasMaxLength(20);

            entity.HasOne(d => d.Batch).WithMany(p => p.QualityChecks)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QualityChecks_Batches");
        });

        modelBuilder.Entity<StockReservation>(entity =>
        {
            entity.HasIndex(e => new { e.BatchId, e.Status }, "IX_StockReservations_BatchId_Status");

            entity.HasIndex(e => e.OrderId, "IX_StockReservations_OrderId");

            entity.HasIndex(e => e.OrderItemId, "IX_StockReservations_OrderItemId");

            entity.HasIndex(e => new { e.ProductId, e.Status }, "IX_StockReservations_ProductId_Status");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_StockReservations_CreatedAt");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 3)");
            entity.Property(e => e.ReservationType).HasMaxLength(30);
            entity.Property(e => e.Status).HasMaxLength(30);

            entity.HasOne(d => d.Batch).WithMany(p => p.StockReservations)
                .HasForeignKey(d => d.BatchId)
                .HasConstraintName("FK_StockReservations_Batches");

            entity.HasOne(d => d.Order).WithMany(p => p.StockReservations)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockReservations_Orders");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.StockReservations)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockReservations_OrderItems");

            entity.HasOne(d => d.Product).WithMany(p => p.StockReservations)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockReservations_Products");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())", "DF_Suppliers_CreatedAt");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.VatNumber).HasMaxLength(30);
        });

        modelBuilder.Entity<SupplierDocument>(entity =>
        {
            entity.Property(e => e.DocumentType).HasMaxLength(50);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.StoragePath).HasMaxLength(500);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysdatetime())", "DF_SupplierDocuments_UploadedAt");
            entity.Property(e => e.UploadedBy).HasMaxLength(100);

            entity.HasOne(d => d.Supplier).WithMany(p => p.SupplierDocuments)
                .HasForeignKey(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierDocuments_Suppliers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
