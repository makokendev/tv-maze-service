namespace CodingChallenge.Api.Mappers;

public interface IMapper<in TSource, out TDestiny>
{
    TDestiny Map(TSource source);
}
