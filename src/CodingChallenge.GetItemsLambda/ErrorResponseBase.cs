namespace CodingChallenge.GetItemsLambda;

public class ErrorResponseBase
{
    public int? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}
