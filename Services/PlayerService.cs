using FootballSimulationApi.DTOs;
using FootballSimulationApi.Models;
using FootballSimulationApi.Repositories;

namespace FootballSimulationApi.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<List<PlayerRatingDto>> GetTopRatingsAsync(int limit = 20)
    {
        return await _playerRepository.GetTopRatedPlayersAsync(limit);
    }

    public async Task<List<PlayerBasicDto>> GetAllBasicPlayersAsync()
    {
        return await _playerRepository.GetAllBasicPlayersAsync();
    }

    public async Task<IEnumerable<PlayerScoutDto>> GetScoutedPlayersAsync(ScoutFilterDto filter)
    {
        var filteredPlayers = await _playerRepository.GetPlayersByScoutFilterAsync(filter);

        return filteredPlayers.Select(p => new PlayerScoutDto
        {
            Name = p.PlayerName,
            Team = p.TeamName,
            Position = p.Position,
            
            Appearances = p.Appearances,
            MinutesPlayed = p.MinutesPlayed,
            
            Goals = p.Goals,
            Assists = p.Assists,
            Xg = p.Xg,
            KeyPasses = p.KeyPasses,
            AccuratePasses = p.AccuratePasses,
            
            Tackles = p.Tackles,
            Interceptions = p.Interceptions,
            Clearances = p.Clearances,
            AerialsWon = p.AerialsWon,
            DuelsWon = p.DuelsWon,
            
            Saves = p.Saves,
            
            Rating = 0 
        }).ToList();
    }

    public async Task<PlayerSeasonStat?> GetPlayerByIdAsync(int id)
    {
        return await _playerRepository.GetPlayerByIdAsync(id);
    }

    public async Task InsertPlayerAsync(PlayerSeasonStat player)
    {
        await _playerRepository.InsertPlayerAsync(player);
    }

    public async Task UpdatePlayerAsync(PlayerSeasonStat player)
    {
        await _playerRepository.UpdatePlayerAsync(player);
    }

    public async Task DeletePlayerAsync(int id)
    {
        await _playerRepository.DeletePlayerAsync(id);
    }

    public async Task<List<TeamStatsDto>> GetTeamStatsAsync()
    {
        return await _playerRepository.GetTeamStatsAsync();
    }

    public async Task<List<TeamRosterDto>> GetPlayersByTeamAsync(string teamName)
    {
        return await _playerRepository.GetPlayersByTeamAsync(teamName);
    }
}
