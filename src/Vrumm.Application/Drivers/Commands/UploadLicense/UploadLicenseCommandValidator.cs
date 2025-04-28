using FluentValidation;

namespace Vrumm.Application.Drivers.Commands.UploadLicense;
public class UploadLicenseCommandValidator : AbstractValidator<UploadLicenseCommand>
{
    private static readonly string[] AllowedExtensions = { ".png", ".bmp" };
    private static readonly string[] AllowedContentTypes = { "image/png", "image/bmp" };
    private const int MaxFileSize = 5 * 1024 * 1024; // 5MB

    public UploadLicenseCommandValidator()
    {
        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage("Driver ID is required.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.")
            .Must(HaveValidExtension).WithMessage($"Invalid file format. Allowed: {string.Join(", ", AllowedExtensions)}");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required.")
            .Must(BeValidContentType).WithMessage($"Invalid content type. Allowed: {string.Join(", ", AllowedContentTypes)}");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("File content is required.")
            .Must(NotExceedMaxFileSize).WithMessage($"File size must not exceed {MaxFileSize / 1024 / 1024}MB.");
    }

    private bool HaveValidExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private bool BeValidContentType(string contentType)
    {
        if (string.IsNullOrEmpty(contentType)) return false;
        return AllowedContentTypes.Contains(contentType.ToLowerInvariant());
    }

    private bool NotExceedMaxFileSize(Stream content)
    {
        return content != null && content.Length <= MaxFileSize;
    }
}
