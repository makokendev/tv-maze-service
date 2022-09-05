using FluentValidation;

namespace CodingChallenge.Application.TVMaze.Queries;

public class GetTVMazeItemByIndexQueryValidator : AbstractValidator<GetTVMazeItemByIndexQuery>
{
    public GetTVMazeItemByIndexQueryValidator()
    {
        RuleFor(v => v.Index)
            .NotEmpty()
            .Custom((index, context) =>
        {
            if (!int.TryParse(index, out _))
            {
                context.AddFailure("Index must be an integer");
            }
        });
    }
}