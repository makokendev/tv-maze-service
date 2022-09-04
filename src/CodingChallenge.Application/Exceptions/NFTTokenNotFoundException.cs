namespace CodingChallenge.Application.Exceptions;

public class TVMazeItemAlreadyExistsException : Exception
{
    public TVMazeItemAlreadyExistsException(string message)
        : base(message)
    {
    }
}
