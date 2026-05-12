using FootballSimulationApi.DTOs;
using FootballSimulationApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballSimulationApi.Repositories;

public interface IPlayerRepository
{
    Task<IEnumerable<PlayerSeasonStat>> GetAllPlayersAsync();
    Task<IEnumerable<PlayerSeasonStat>> GetPlayersByScoutFilterAsync(ScoutFilterDto filter);
    Task<List<PlayerRatingDto>> GetTopRatedPlayersAsync(int limit);
    Task<PlayerSeasonStat?> GetPlayerByIdAsync(int id);
    Task InsertPlayerAsync(PlayerSeasonStat player);
    Task UpdatePlayerAsync(PlayerSeasonStat player);
    Task DeletePlayerAsync(int id);
    Task<List<PlayerSeasonStat>> GetPlayersByNamesAsync(List<string> playerNames);
    Task<List<TeamRosterDto>> GetPlayersByTeamAsync(string teamName);
    Task<List<TeamStatsDto>> GetTeamStatsAsync();
    Task<List<PlayerBasicDto>> GetAllBasicPlayersAsync();
}
