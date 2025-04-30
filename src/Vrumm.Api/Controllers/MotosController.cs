using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vrumm.Api.Models;
using Vrumm.Api.Models.Motos;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
using Vrumm.Application.Motorcycles.Commands.DeleteMotorcycle;
using Vrumm.Application.Motorcycles.Commands.UpdateMotorcycle;
using Vrumm.Application.Motorcycles.Dtos;
using Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
using Vrumm.Domain.Entities;

namespace Vrumm.Api.Controllers;
[ApiController]
[Route("motos")]
public class MotosController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public MotosController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher
            ?? throw new ArgumentNullException(nameof(commandDispatcher));
        
        _queryDispatcher = queryDispatcher
            ?? throw new ArgumentNullException(nameof(queryDispatcher));
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateMoto(
        [FromBody] CreateMotoRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToCommand(request);

        var result = await _commandDispatcher
            .DispatchAsync<CreateMotorcycleCommand, Guid>(command, cancellationToken);

        return CreatedAtAction(nameof(CreateMoto), new { id = result.ToString() }, null);
    }

    [HttpGet]
    [Authorize(Roles = UserRole.Admin)]
    [ProducesResponseType(typeof(IReadOnlyList<MotoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<MotoResponse>>> GetMotos(
        [FromQuery] GetMotosRequest request,
        CancellationToken cancellationToken)
    {
        var query = MapToQuery(request);

        var result = await _queryDispatcher
            .DispatchAsync<GetMotorcyclesQuery, PaginatedList<MotorcycleDto>>(query, cancellationToken);

        if (!result.Items.Any())
            return NotFound(new ErrorResponse("Moto não encontrada"));

        var response = MapToResponse(result);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Entregador}")]
    [ProducesResponseType(typeof(MotoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MotoResponse>> GetMotoById(
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetMotorcyclesQuery
        {
            MotorcycleId = Guid.Parse(id)
        };

        var result = await _queryDispatcher
            .DispatchAsync<GetMotorcyclesQuery, PaginatedList<MotorcycleDto>>
            (query, cancellationToken);

        if (!result.Items.Any())
            return NotFound(new ErrorResponse("Moto não encontrada"));

        var response = MapToMotoResponse(result);
        return Ok(response);
    }

    [HttpPut("{id}/placa")]
    [Authorize(Roles = UserRole.Admin)]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpdateResponse>> UpdateMotoPlaca(
        string id,
        [FromBody] UpdateMotoPlacaRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToUpdateCommand(id, request);
        
        await _commandDispatcher
            .DispatchAsync(command, cancellationToken);

        return Ok(new UpdateResponse("Placa modificada com sucesso"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = UserRole.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteMoto(
        string id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteMotorcycleCommand { Id = Guid.Parse(id) };

        await _commandDispatcher
            .DispatchAsync(command, cancellationToken);

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
            Id = Guid.Parse(request.Identificador),
            Model = request.Modelo,
            Year = request.Ano,
            LicensePlate = request.Placa
        };
    }

    private UpdateMotorcycleCommand MapToUpdateCommand
        (string id, UpdateMotoPlacaRequest request)
    {
        return new UpdateMotorcycleCommand
        {
            Id = Guid.Parse(id),
            LicensePlate = request.Placa
        };
    }

    private IReadOnlyList<MotoResponse> MapToResponse
        (PaginatedList<MotorcycleDto> result)
    {
        return result.Items.Select(dto => new MotoResponse
        {
            Identificador = dto.Id.ToString(),
            Ano = dto.Year,
            Modelo = dto.Model,
            Placa = dto.LicensePlate
        }).ToList();
    }

    private MotoResponse MapToMotoResponse(PaginatedList<MotorcycleDto> pMotorcycle)
    {
        var motorcycle = pMotorcycle.Items.FirstOrDefault();

        return new MotoResponse
        {
            Identificador = motorcycle.Id.ToString(),
            Ano = motorcycle.Year,
            Modelo = motorcycle.Model,
            Placa = motorcycle.LicensePlate
        };
    }
}