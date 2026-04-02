using Mandorle.Application.Traceability.Commands;
using Mandorle.Application.Traceability.Mapping;
using Mandorle.Application.Traceability.Models;
using Mandorle.Domain.Entities;
using Mandorle.Domain.Interfaces;
using MediatR;

namespace Mandorle.Application.Traceability.Handlers;

public class UpsertPublicTraceViewCommandHandler : IRequestHandler<UpsertPublicTraceViewCommand, PublicTraceViewDto>
{
    private readonly IPublicTraceViewRepository _publicTraceViewRepository;
    private readonly IBatchRepository _batchRepository;

    public UpsertPublicTraceViewCommandHandler(IPublicTraceViewRepository publicTraceViewRepository, IBatchRepository batchRepository)
    {
        _publicTraceViewRepository = publicTraceViewRepository;
        _batchRepository = batchRepository;
    }

    public async Task<PublicTraceViewDto> Handle(UpsertPublicTraceViewCommand request, CancellationToken cancellationToken)
    {
        var batchCode = request.BatchCode?.Trim()
            ?? throw new InvalidOperationException("Batch code is required.");
        var productName = request.ProductName?.Trim()
            ?? throw new InvalidOperationException("Product name is required.");

        var batch = await _batchRepository.GetByIdAsync(request.BatchId, cancellationToken)
            ?? throw new InvalidOperationException("The selected batch does not exist.");

        if (!string.Equals(batch.BatchCode, batchCode, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("The batch code does not match the selected batch.");
        }

        var traceView = await _publicTraceViewRepository.GetByBatchIdAsync(request.BatchId, cancellationToken);

        if (traceView is null)
        {
            traceView = new PublicTraceView
            {
                BatchId = request.BatchId,
                BatchCode = batchCode,
                ProductName = productName,
                BioFlag = request.BioFlag,
                OriginInfo = request.OriginInfo,
                MainDates = request.MainDates,
                LastUpdatedAt = DateTime.UtcNow
            };

            await _publicTraceViewRepository.AddAsync(traceView, cancellationToken);
        }
        else
        {
            traceView.BatchCode = batchCode;
            traceView.ProductName = productName;
            traceView.BioFlag = request.BioFlag;
            traceView.OriginInfo = request.OriginInfo;
            traceView.MainDates = request.MainDates;
            traceView.LastUpdatedAt = DateTime.UtcNow;

            _publicTraceViewRepository.Update(traceView);
        }

        await _publicTraceViewRepository.SaveChangesAsync(cancellationToken);
        return traceView.ToDto();
    }
}
