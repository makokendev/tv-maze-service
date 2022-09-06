namespace CodingChallenge.GetItemsLambda;

public interface IResponse
{
    public bool IsSuccess { get; set; }
    public ErrorResponseBase? ErrorResponseInfo { get; set; }
}
