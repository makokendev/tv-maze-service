namespace CodingChallenge.GetItemsLambda;

public class ErrorResponse : IResponse
{
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public ErrorResponseBase? ErrorResponseInfo { get; set; }
}
