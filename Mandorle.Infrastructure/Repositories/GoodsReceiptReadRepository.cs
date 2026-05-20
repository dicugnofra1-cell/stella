using Mandorle.Application.GoodsReceipts.Abstractions;
using Mandorle.Application.GoodsReceipts.Models;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mandorle.Infrastructure.Repositories;

public class GoodsReceiptReadRepository : IGoodsReceiptReadRepository
{
    private readonly StellaFruttaDbContext _context;

    public GoodsReceiptReadRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<GoodsReceiptRowDto>> GetTodayAsync(DateOnly day, string? search, CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            procedureName: "dbo.usp_GoodsReceipts_Today",
            configureParameters: command =>
            {
                var dayParameter = command.CreateParameter();
                dayParameter.ParameterName = "@Day";
                dayParameter.DbType = DbType.Date;
                dayParameter.Value = day.ToDateTime(TimeOnly.MinValue);
                command.Parameters.Add(dayParameter);

                AddSearchParameter(command, search);
            },
            cancellationToken);
    }

    public async Task<IReadOnlyList<GoodsReceiptRowDto>> GetActiveHistoryAsync(string? search, CancellationToken cancellationToken = default)
    {
        return await ExecuteStoredProcedureAsync(
            procedureName: "dbo.usp_GoodsReceipts_ActiveHistory",
            configureParameters: command => AddSearchParameter(command, search),
            cancellationToken);
    }

    private async Task<IReadOnlyList<GoodsReceiptRowDto>> ExecuteStoredProcedureAsync(
        string procedureName,
        Action<System.Data.Common.DbCommand> configureParameters,
        CancellationToken cancellationToken)
    {
        var results = new List<GoodsReceiptRowDto>();
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            configureParameters(command);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(new GoodsReceiptRowDto(
                    BatchId: reader.GetInt32(reader.GetOrdinal("BatchId")),
                    ProductId: reader.GetInt32(reader.GetOrdinal("ProductId")),
                    BatchCode: reader.GetString(reader.GetOrdinal("BatchCode")),
                    CreatedAt: reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    SupplierName: reader.GetString(reader.GetOrdinal("SupplierName")),
                    ProductName: reader.GetString(reader.GetOrdinal("ProductName")),
                    BatchType: reader.GetString(reader.GetOrdinal("BatchType")),
                    BioFlag: reader.GetBoolean(reader.GetOrdinal("BioFlag")),
                    Quantity: reader.GetDecimal(reader.GetOrdinal("Quantity")),
                    PurchaseUnitPrice: ReadNullableDecimal(reader, "PurchaseUnitPrice"),
                    UnitOfMeasure: reader.GetString(reader.GetOrdinal("UnitOfMeasure")),
                    Status: reader.GetString(reader.GetOrdinal("Status")),
                    Variety: ReadNullableString(reader, "Variety"),
                    Notes: ReadNullableString(reader, "Notes"),
                    Operator: reader.GetString(reader.GetOrdinal("Operator")),
                    SupplierId: ReadNullableInt32(reader, "SupplierId"),
                    CertificationId: ReadNullableInt32(reader, "CertificationId")));
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

    private static void AddSearchParameter(System.Data.Common.DbCommand command, string? search)
    {
        var searchParameter = command.CreateParameter();
        searchParameter.ParameterName = "@Search";
        searchParameter.DbType = DbType.String;
        searchParameter.Value = (object?)Normalize(search) ?? DBNull.Value;
        command.Parameters.Add(searchParameter);
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

    private static int? ReadNullableInt32(IDataRecord reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }

    private static decimal? ReadNullableDecimal(IDataRecord reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetDecimal(ordinal);
    }
}
