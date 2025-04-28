using Microsoft.Extensions.Logging;
using Vrumm.Application.Motorcycles.Dtos;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Motorcycles.Queries.GetMotorcycleRegistrationEvents;
public class GetMotorcycleRegistrationEventsHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetMotorcycleRegistrationEventsHandler> _logger;

    public GetMotorcycleRegistrationEventsHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetMotorcycleRegistrationEventsHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<MotorcycleRegistrationEventDto>> Handle(
        GetMotorcycleRegistrationEventsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting motorcycle registration events{YearFilter}",
                request.Year.HasValue ? $" for year {request.Year}" : "");

            var events = request.Year.HasValue
                ? await _unitOfWork.MotorcycleRegistrationEvents.GetByYearAsync(request.Year.Value, cancellationToken)
                : await _unitOfWork.MotorcycleRegistrationEvents.GetAllAsync(cancellationToken);

            var dtos = events.Select(e => new MotorcycleRegistrationEventDto
            {
                Id = e.Id,
                MotorcycleId = e.MotorcycleId,
                Year = e.Year,
                Model = e.Model,
                LicensePlate = e.LicensePlate,
                EventTimestamp = e.EventTimestamp,
                ProcessedAt = e.ProcessedAt
            });

            _logger.LogInformation("Retrieved {Count} motorcycle registration events", dtos.Count());
            return dtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving motorcycle registration events: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}