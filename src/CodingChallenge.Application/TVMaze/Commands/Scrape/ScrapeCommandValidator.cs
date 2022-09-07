using FluentValidation;

namespace CodingChallenge.Application.TVMaze.Commands.Scrape;

public class ScrapeCommandValidator : AbstractValidator<ScrapeCommand>
{
    public ScrapeCommandValidator()
    {
    }
}

