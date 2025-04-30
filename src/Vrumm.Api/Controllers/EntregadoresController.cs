using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vrumm.Api.Models;
using Vrumm.Api.Models.Entregador;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Drivers.Commands.CreateDriver;
using Vrumm.Application.Drivers.Commands.UploadLicense;
using Vrumm.Domain.Entities;

namespace Vrumm.Api.Controllers;
[ApiController]
[Route("entregadores")]
public class EntregadoresController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public EntregadoresController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher
            ?? throw new ArgumentNullException(nameof(commandDispatcher));
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateEntregador
        ([FromBody] CreateEntregadorRequest request, CancellationToken cancellationToken)
    {
        var command = MapToCommand(request);
        await _commandDispatcher
            .DispatchAsync<CreateDriverCommand, Guid>(command, cancellationToken);

        return CreatedAtAction(
            nameof(CreateEntregador),
            new { id = request.Identificador }, null);
    }

    [HttpPost("{id}/cnh")]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Entregador}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadCnh(
        string id,
        [FromBody] CnhUploadRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToUploadCommand(id, request);

        var result = await _commandDispatcher
            .DispatchAsync<UploadLicenseCommand, string>(command, cancellationToken);

        return CreatedAtAction(nameof(UploadCnh), new { id }, result);
    }

    private CreateDriverCommand MapToCommand(CreateEntregadorRequest request)
    {
        return new CreateDriverCommand
        {
            Id = Guid.Parse(request.Identificador),
            Name = request.Nome,
            Cnpj = request.Cnpj,
            BirthDate = request.DataNascimento,
            LicenseNumber = request.NumeroCnh,
            LicenseType = request.TipoCnh,
            LicenseImageBase64 = request.ImagemCnh
        };
    }

    private UploadLicenseCommand MapToUploadCommand(string id, CnhUploadRequest request)
    {
        var bytes = Convert.FromBase64String(request.ImagemCnh);
        return new UploadLicenseCommand
        {
            DriverId = Guid.Parse(id),
            FileName = $"{id}_cnh.png",
            ContentType = "image/png",
            Content = new MemoryStream(bytes)
        };
    }
}