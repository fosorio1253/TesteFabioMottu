using Microsoft.AspNetCore.Mvc;
using Vrumm.Api.Models;
using Vrumm.Api.Models.Locacao;
using Vrumm.Api.Models.Motos;
using Vrumm.Application.Plans;
using Vrumm.Application.Rentals.Commands.CreateRental;
using Vrumm.Application.Rentals.Commands.FinalizeRental;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Messaging.Abstractions;

namespace Vrumm.Api.Controllers;
[ApiController]
[Route("locacao")]
public class LocacaoController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPlanFactory _planFactory;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<LocacaoController> _logger;

    public LocacaoController(
        IUnitOfWork unitOfWork,
        IPlanFactory planFactory,
        IMessagePublisher messagePublisher,
        ILogger<LocacaoController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _planFactory = planFactory ?? throw new ArgumentNullException(nameof(planFactory));
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateLocacao(
        [FromBody] CreateLocacaoRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToCommand(request);
        var handler = new CreateRentalCommandHandler(_unitOfWork, _planFactory, _messagePublisher, _logger);
        await handler.Handle(command, cancellationToken);
        return CreatedAtAction(nameof(GetLocacaoById), new { id = command.RentalId }, null);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LocacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocacaoResponse>> GetLocacaoById(
        string id,
        CancellationToken cancellationToken)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(id, cancellationToken);
        if (rental == null)
        {
            return NotFound(new ErrorResponse("Locação não encontrada"));
        }

        var response = MapToResponse(rental);
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
        var handler = new FinalizeRentalCommandHandler(_unitOfWork, _messagePublisher, _logger);
        await handler.Handle(command, cancellationToken);
        return Ok(new UpdateResponse("Data de devolução informada com sucesso"));
    }

    private CreateRentalCommand MapToCommand(CreateLocacaoRequest request)
    {
        return new CreateRentalCommand
        {
            RentalId = Guid.NewGuid().ToString(),
            MotorcycleId = request.MotoId,
            DriverId = request.EntregadorId,
            PlanId = request.Plano,
            StartDate = request.DataInicio,
            ExpectedEndDate = request.DataPrevisaoTermino
        };
    }

    private FinalizeRentalCommand MapToFinalizeCommand(Guid id, LocacaoDevolucaoRequest request)
    {
        return new FinalizeRentalCommand
        {
            RentalId = id,
            ReturnDate = request.DataDevolucao
        };
    }

    private LocacaoResponse MapToResponse(Rental rental)
    {
        var plan = _planFactory.CreatePlan(rental.PlanId);
        return new LocacaoResponse
        {
            Identificador = rental.Id,
            ValorDiaria = plan.DailyRate,
            EntregadorId = rental.DriverId,
            MotoId = rental.MotorcycleId,
            DataInicio = rental.StartDate,
            DataTermino = rental.EndDate ?? rental.ExpectedEndDate,
            DataPrevisaoTermino = rental.ExpectedEndDate,
            DataDevolucao = rental.EndDate
        };
    }
}