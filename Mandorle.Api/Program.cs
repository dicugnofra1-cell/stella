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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
