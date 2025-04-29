using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Vrumm.Api.Models;
using Vrumm.Api.Models.Entregador;
using Vrumm.Application.Drivers.Commands.CreateDriver;
using Vrumm.Application.Drivers.Commands.UploadLicense;
using Vrumm.Infrastructure.Data.UnitOfWork;
using Vrumm.Infrastructure.Storage.Abstractions;

namespace Vrumm.Api.Controllers;
[ApiController]
[Route("entregadores")]
public class EntregadoresController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private readonly IOptions<StorageOptions> _storageOptions;
    private readonly ILogger<EntregadoresController> _logger;

    public EntregadoresController(
        IUnitOfWork unitOfWork,
        IStorageService storageService,
        IOptions<StorageOptions> storageOptions,
        ILogger<EntregadoresController> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _storageOptions = storageOptions ?? throw new ArgumentNullException(nameof(storageOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateEntregador(
        [FromBody] CreateEntregadorRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToCommand(request);
        var handler = new CreateDriverCommandHandler(_unitOfWork, _logger);
        await handler.Handle(command, cancellationToken);
        return CreatedAtAction(nameof(CreateEntregador), new { id = request.Identificador }, null);
    }

    [HttpPost("{id}/cnh")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UploadCnh(
        string id,
        [FromBody] CnhUploadRequest request,
        CancellationToken cancellationToken)
    {
        var command = MapToUploadCommand(id, request);
        var handler = new UploadLicenseCommandHandler(_unitOfWork, _storageService, _storageOptions, _logger);
        await handler.Handle(command, cancellationToken);
        return CreatedAtAction(nameof(UploadCnh), new { id }, null);
    }

    private CreateDriverCommand MapToCommand(CreateEntregadorRequest request)
    {
        return new CreateDriverCommand
        {
            Id = request.Identificador,
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
            DriverId = id,
            FileName = $"{id}_cnh.png",
            ContentType = "image/png",
            Content = new MemoryStream(bytes)
        };
    }
}