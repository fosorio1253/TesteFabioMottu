using Microsoft.AspNetCore.Mvc;
using Vrumm.Api.Models;
using Vrumm.Api.Models.Locacao;
using Vrumm.Api.Models.Motos;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Rentals.Commands.CreateRental;
using Vrumm.Application.Rentals.Commands.FinalizeRental;
using Vrumm.Application.Rentals.Dtos;
using Vrumm.Application.Rentals.Queries.GetRentals;

namespace Vrumm.Api.Controllers;
[ApiController]
[Route("locacao")]
public class LocacaoController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public LocacaoController(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher
            ?? throw new ArgumentNullException(nameof(commandDispatcher));

        _queryDispatcher = queryDispatcher
            ?? throw new ArgumentNullException(nameof(queryDispatcher));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateLocacao(
        [FromBody] CreateLocacaoRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToCommand(request);

        var result = await _commandDispatcher
            .DispatchAsync<CreateRentalCommand, Guid>(command, cancellationToken);

        return CreatedAtAction(nameof(GetLocacaoById), new { id = result }, null);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LocacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocacaoResponse>> GetLocacaoById(
        string id,
        CancellationToken cancellationToken)
    {
        var query = MapToGetRentalQuery(id);

        var result = await _queryDispatcher
            .DispatchAsync<GetRentalsQuery, PaginatedList<RentalDto>>(query, cancellationToken);
                
        if (!result.Items.Any())
            return NotFound(new ErrorResponse("Locação não encontrada"));

        var response = MapToResponse(result);
        return Ok(response);
    }

    [HttpPut("{id}/devolucao")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UpdateResponse>> FinalizeLocacao(
        string id,
        [FromBody] LocacaoDevolucaoRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToFinalizeCommand(id, request);

        await _commandDispatcher
            .DispatchAsync(command, cancellationToken);

        return Ok(new UpdateResponse("Data de devolução informada com sucesso"));
    }

    private CreateRentalCommand MapToCommand(CreateLocacaoRequest request)
    {
        return new CreateRentalCommand
        {
            MotorcycleId = Guid.Parse(request.MotoId),
            DriverId = Guid.Parse(request.EntregadorId),
            PlanId = request.Plano,
            StartDate = request.DataInicio,
            ExpectedEndDate = request.DataPrevisaoTermino
        };
    }

    private GetRentalsQuery MapToGetRentalQuery(string id)
    {
        return new GetRentalsQuery
        {
            RentalId = Guid.Parse(id)
        };
    }

    private FinalizeRentalCommand MapToFinalizeCommand(string id, LocacaoDevolucaoRequest request)
    {
        return new FinalizeRentalCommand
        {
            RentalId = Guid.Parse(id),
            ReturnDate = request.DataDevolucao
        };
    }

    private LocacaoResponse MapToResponse(PaginatedList<RentalDto> pRental)
    {
        var rental = pRental.Items.FirstOrDefault();

        return new LocacaoResponse
        {
            Identificador = rental.Id.ToString(),
            ValorDiaria = rental.Plan.DailyRate,
            EntregadorId = rental.DriverId.ToString(),
            MotoId = rental.MotorcycleId.ToString(),
            DataInicio = rental.StartDate,
            DataTermino = rental.EndDate ?? rental.ExpectedEndDate,
            DataPrevisaoTermino = rental.ExpectedEndDate,
            DataDevolucao = rental.EndDate
        };
    }
}