using System.Data;
using Mandorle.Application.Batches.Abstractions;
using Mandorle.Application.Batches.Models;
using Mandorle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Mandorle.Infrastructure.Repositories;

public class ActiveBatchReadRepository : IActiveBatchReadRepository
{
    private readonly StellaFruttaDbContext _context;

    public ActiveBatchReadRepository(StellaFruttaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ActiveBatchRowDto>> SearchActiveAsync(string? search, CancellationToken cancellationToken = default)
    {
        var results = new List<ActiveBatchRowDto>();
        var connection = _context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "dbo.usp_Batches_Active";
            command.CommandType = CommandType.StoredProcedure;

            var searchParameter = command.CreateParameter();
            searchParameter.ParameterName = "@Search";
            searchParameter.DbType = DbType.String;
            searchParameter.Value = (object?)Normalize(search) ?? DBNull.Value;
            command.Parameters.Add(searchParameter);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(new ActiveBatchRowDto(
                    BatchId: reader.GetInt32(reader.GetOrdinal("BatchId")),
                    BatchCode: reader.GetString(reader.GetOrdinal("BatchCode")),
                    ProductId: reader.GetInt32(reader.GetOrdinal("ProductId")),
                    ProductName: reader.GetString(reader.GetOrdinal("ProductName")),
                    BatchType: reader.GetString(reader.GetOrdinal("BatchType")),
                    Status: reader.GetString(reader.GetOrdinal("Status")),
                    BioFlag: reader.GetBoolean(reader.GetOrdinal("BioFlag")),
                    Variety: ReadNullableString(reader, "Variety"),
                    PhysicalStock: reader.GetDecimal(reader.GetOrdinal("PhysicalStock")),
                    ReservedStock: reader.GetDecimal(reader.GetOrdinal("ReservedStock")),
                    AvailableStock: reader.GetDecimal(reader.GetOrdinal("AvailableStock")),
                    UnitOfMeasure: reader.GetString(reader.GetOrdinal("UnitOfMeasure")),
                    SupplierName: reader.GetString(reader.GetOrdinal("SupplierName")),
                    SupplierId: ReadNullableInt32(reader, "SupplierId"),
                    QrActive: reader.GetBoolean(reader.GetOrdinal("QrActive")),
                    CreatedAt: reader.GetDateTime(reader.GetOrdinal("CreatedAt"))));
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

    private static int? ReadNullableInt32(IDataRecord reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }
}
