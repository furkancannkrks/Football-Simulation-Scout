using FootballSimulationApi.DTOs;
using FootballSimulationApi.Models;

namespace FootballSimulationApi.Services;

public interface IPlayerService
{
    Task<List<PlayerRatingDto>> GetTopRatingsAsync(int limit = 20);
    Task<IEnumerable<PlayerScoutDto>> GetScoutedPlayersAsync(ScoutFilterDto filter);
    Task<PlayerSeasonStat?> GetPlayerByIdAsync(int id);
    Task InsertPlayerAsync(PlayerSeasonStat player);
    Task UpdatePlayerAsync(PlayerSeasonStat player);
    Task DeletePlayerAsync(int id);
    Task<List<TeamRosterDto>> GetPlayersByTeamAsync(string teamName);
    Task<List<TeamStatsDto>> GetTeamStatsAsync();
    Task<List<PlayerBasicDto>> GetAllBasicPlayersAsync();
}
