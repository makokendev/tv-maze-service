﻿using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Domain.Entities;
using CodingChallenge.Domain.Entities.TvMaze;

namespace CodingChallenge.Application.Interfaces;
public interface ITVMazeRecordRepository
{
    Task<TVMazeCastDataResponse> GetTVMazeCastById(int id);
    //Task<List<TVMazeRecordEntity>> GetByWalletIdAsync(string walletId);
    Task<TVMazeRecordEntity> GetByIndexAsync(string index);
    Task AddScrapeTaskAsync(AddScrapeTaskCommand command);
    Task<TVMazeCastDataResponse> ScrapeAsync(int index);
   
}
