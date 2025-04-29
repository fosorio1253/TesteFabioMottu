using Microsoft.AspNetCore.Mvc;
using Vrumm.Api.Models;
using Vrumm.Api.Models.Motos;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
using Vrumm.Application.Motorcycles.Commands.DeleteMotorcycle;
using Vrumm.Application.Motorcycles.Commands.UpdateMotorcycle;
using Vrumm.Application.Motorcycles.Dtos;
using Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Api.Controllers;
[ApiController]
[Route("motos")]
public class MotosController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<MotosController> _logger;

    public MotosController(
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher,
        ILogger<MotosController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateMoto(
        [FromBody] CreateMotoRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToCommand(request);
        var handler = new CreateMotorcycleCommandHandler(_unitOfWork, _messagePublisher, _logger);
        await handler.Handle(command, cancellationToken);
        return CreatedAtAction(nameof(GetMotoById), new { id = request.Identificador }, null);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MotoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<MotoResponse>>> GetMotos(
        [FromQuery] GetMotosRequest request,
        CancellationToken cancellationToken)
    {
        var query = MapToQuery(request);
        var result = await new GetMotorcyclesQueryHandler(_unitOfWork, _logger).Handle(query, cancellationToken);
        var response = MapToResponse(result);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MotoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MotoResponse>> GetMotoById(
        string id,
        CancellationToken cancellationToken)
    {
        var motorcycle = await _unitOfWork.Motorcycles.GetByIdAsync(id, cancellationToken);
        if (motorcycle == null)
        {
            return NotFound(new ErrorResponse("Moto não encontrada"));
        }

        var response = MapToMotoResponse(motorcycle);
        return Ok(response);
    }

    [HttpPut("{id}/placa")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpdateResponse>> UpdateMotoPlaca(
        string id,
        [FromBody] UpdateMotoPlacaRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToUpdateCommand(id, request);
        var handler = new UpdateMotorcycleCommandHandler(_unitOfWork, _logger);
        await handler.Handle(command, cancellationToken);
        return Ok(new UpdateResponse("Placa modificada com sucesso"));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteMoto(
        string id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMotorcycleCommand { Id = id };
        var handler = new DeleteMotorcycleCommandHandler(_unitOfWork, _logger);
        await handler.Handle(command, cancellationToken);
        return Ok();
    }

    private GetMotorcyclesQuery MapToQuery(GetMotosRequest request)
    {
        return new GetMotorcyclesQuery
        {
            LicensePlate = request.Placa
        };
    }

    private CreateMotorcycleCommand MapToCommand(CreateMotoRequest request)
    {
        return new CreateMotorcycleCommand
        {
            Id = request.Identificador,
            Model = request.Modelo,
            Year = request.Ano,
            LicensePlate = request.Placa
        };
    }

    private UpdateMotorcycleCommand MapToUpdateCommand(string id, UpdateMotoPlacaRequest request)
    {
        return new UpdateMotorcycleCommand
        {
            Id = id,
            LicensePlate = request.Placa
        };
    }

    private IReadOnlyList<MotoResponse> MapToResponse(PaginatedList<MotorcycleDto> result)
    {
        return result.Items.Select(dto => new MotoResponse
        {
            Identificador = dto.Id,
            Ano = dto.Year,
            Modelo = dto.Model,
            Placa = dto.LicensePlate
        }).ToList();
    }

    private MotoResponse MapToMotoResponse(Motorcycle motorcycle)
    {
        return new MotoResponse
        {
            Identificador = motorcycle.Id,
            Ano = motorcycle.Details().Year().ToInt(),
            Modelo = motorcycle.Details().Model().ToStringRepresentation(),
            Placa = motorcycle.Details().LicensePlate().ToStringRepresentation()
        };
    }
}