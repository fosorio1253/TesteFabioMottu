namespace Vrumm.Infrastructure.Storage.Abstractions;
public class StorageFile
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public Stream Content { get; set; }

    public StorageFile(string fileName, string contentType, Stream content)
    {
        FileName = fileName;
        ContentType = contentType;
        Content = content;
    }
}