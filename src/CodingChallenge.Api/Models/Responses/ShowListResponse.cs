using System.ComponentModel.DataAnnotations;

namespace CodingChallenge.Api.Models.Responses;

public class ShowListResponse
{
    [Required]
    public IEnumerable<ShowResponse> Shows { get; set; } = Enumerable.Empty<ShowResponse>();
}

public class ShowResponse
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public IEnumerable<CastMemberResponse> Cast { get; set; } = Enumerable.Empty<CastMemberResponse>();
}

public class CastMemberResponse
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? BirthDate { get; set; }
}

