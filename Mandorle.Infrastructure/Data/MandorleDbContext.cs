using System;
using System.Collections.Generic;
using Mandorle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Data;

public partial class MandorleDbContext : DbContext
{
    public MandorleDbContext()
    {
    }

    public MandorleDbContext(DbContextOptions<MandorleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Batch> Batches { get; set; }

    public virtual DbSet<InventoryMovement> InventoryMovements { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MandorleDB;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Batches__3214EC07DA0DD324");

            entity.HasIndex(e => e.BatchCode, "UQ__Batches__B22ADA8E7A00E4C6").IsUnique();

            entity.Property(e => e.BatchCode).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Type).HasMaxLength(20);

            entity.HasOne(d => d.ParentBatch).WithMany(p => p.InverseParentBatch)
                .HasForeignKey(d => d.ParentBatchId)
                .HasConstraintName("FK__Batches__ParentB__6383C8BA");

            entity.HasOne(d => d.Product).WithMany(p => p.Batches)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Batches__Product__628FA481");
        });

        modelBuilder.Entity<InventoryMovement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC0798F9D9E3");

            entity.Property(e => e.MovementDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.MovementType).HasMaxLength(50);
            entity.Property(e => e.Quantity).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Reference).HasMaxLength(100);

            entity.HasOne(d => d.Batch).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Batch__68487DD7");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryMovements)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__6754599E");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC072B86BA5B");

            entity.HasIndex(e => e.Sku, "UQ__Products__CA1ECF0DA4869A75").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
