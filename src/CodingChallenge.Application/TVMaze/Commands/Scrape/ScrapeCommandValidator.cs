using CodingChallenge.Application.TVMaze.Commands.Scrape;
using FluentValidation;

namespace CodingChallenge.Application.TVMaze.Commands.Mint;

public class ScrapeCommandValidator : AbstractValidator<ScrapeCommand>
{
    public ScrapeCommandValidator()
    {
    }
}

