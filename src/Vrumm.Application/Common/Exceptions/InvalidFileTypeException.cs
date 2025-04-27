namespace Vrumm.Application.Common.Exceptions;
public class InvalidFileTypeException : Exception
{
    public InvalidFileTypeException(string fileType)
        : base($"The file type \"{fileType}\" is not allowed.")
    {
    }
}