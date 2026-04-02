using Mandorle.Application;
using Mandorle.Domain.Interfaces;
using Mandorle.Infrastructure.Data;
using Mandorle.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("StellaFruttaDb")
    ?? throw new InvalidOperationException("Connection string 'StellaFruttaDb' not found.");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StellaFruttaDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IBatchRepository, BatchRepository>();
builder.Services.AddScoped<IBatchLinkRepository, BatchLinkRepository>();
builder.Services.AddScoped<IInventoryMovementRepository, InventoryMovementRepository>();
builder.Services.AddScoped<INonConformityRepository, NonConformityRepository>();
builder.Services.AddScoped<IPublicTraceViewRepository, PublicTraceViewRepository>();
builder.Services.AddScoped<IQualityCheckRepository, QualityCheckRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();
builder.Services.AddScoped<ISupplierDocumentRepository, SupplierDocumentRepository>();
builder.Services.AddScoped<IStockReservationRepository, StockReservationRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
