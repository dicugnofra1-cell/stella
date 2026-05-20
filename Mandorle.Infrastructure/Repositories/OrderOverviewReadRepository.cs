using System.Data;
using Mandorle.Application.Orders.Abstractions;
using Mandorle.Application.Orders.Models;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class OrderOverviewReadRepository : IOrderOverviewReadRepository
{
    private readonly StellaFruttaDbContext _context;

    public OrderOverviewReadRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrderOverviewRowDto>> SearchAsync(string? orderType, string? search, CancellationToken cancellationToken = default)
    {
        var results = new List<OrderOverviewRowDto>();
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.usp_Orders_Overview";
            command.CommandType = CommandType.StoredProcedure;

            var orderTypeParameter = command.CreateParameter();
            orderTypeParameter.ParameterName = "@OrderType";
            orderTypeParameter.DbType = DbType.String;
            orderTypeParameter.Value = (object?)Normalize(orderType)?.ToUpperInvariant() ?? DBNull.Value;
            command.Parameters.Add(orderTypeParameter);

            var searchParameter = command.CreateParameter();
            searchParameter.ParameterName = "@Search";
            searchParameter.DbType = DbType.String;
            searchParameter.Value = (object?)Normalize(search) ?? DBNull.Value;
            command.Parameters.Add(searchParameter);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(new OrderOverviewRowDto(
                    OrderId: reader.GetInt32(reader.GetOrdinal("OrderId")),
                    OrderCode: reader.GetString(reader.GetOrdinal("OrderCode")),
                    CreatedAt: reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    CustomerId: reader.GetInt32(reader.GetOrdinal("CustomerId")),
                    CustomerName: reader.GetString(reader.GetOrdinal("CustomerName")),
                    CustomerType: reader.GetString(reader.GetOrdinal("CustomerType")),
                    OrderType: reader.GetString(reader.GetOrdinal("OrderType")),
                    Status: reader.GetString(reader.GetOrdinal("Status")),
                    PaymentStatus: ReadNullableString(reader, "PaymentStatus"),
                    LotCodes: ReadNullableString(reader, "LotCodes"),
                    TotalAmount: reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    Currency: reader.GetString(reader.GetOrdinal("Currency")),
                    ItemCount: reader.GetInt32(reader.GetOrdinal("ItemCount")),
                    InvoiceDocumentNumber: ReadNullableString(reader, "InvoiceDocumentNumber"),
                    InvoiceDocumentType: ReadNullableString(reader, "InvoiceDocumentType")));
            }
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }

        return results;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? ReadNullableString(IDataRecord reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }
}
