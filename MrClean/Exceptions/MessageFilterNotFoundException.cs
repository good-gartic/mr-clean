namespace MrClean.Exceptions;

public class MessageFilterNotFoundException : ApplicationException
{
    public MessageFilterNotFoundException() : base("A filter with the specified ID couldn't be found in the database.")
    {
    }
}