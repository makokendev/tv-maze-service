using CodingChallenge.Domain.Base;

namespace CodingChallenge.Domain.Entities.TvMaze;

public class TVMazeRecordEntity : AuditableEntity
{
    public int Index { get; set; }
    public string ProductionType { get; set; } = string.Empty;
    public List<TVMazeCastItem> CastList { get; set; } = new List<TVMazeCastItem>();
}
