namespace Vrumm.Domain.Exceptions.Drivers;
public class FileUploadException : DomainException
{
    public string FileName { get; }

    public FileUploadException(string message)
        : base(message)
    {
    }

    public FileUploadException(string message, Exception innerException)
        : base(message, innerException)
    {
        if (innerException is IOException fileEx)
            FileName = fileEx.Source;
    }

    public FileUploadException(string fileName, string message)
        : base(message)
    {
        FileName = fileName;
    }

    public FileUploadException(string fileName, string message, Exception innerException)
        : base(message, innerException)
    {
        FileName = fileName;
    }
}