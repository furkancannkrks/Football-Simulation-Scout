using FootballSimulationApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballSimulationApi.Services;

public interface IPlayerRatingService
{
    Task<IEnumerable<PlayerRatingDto>> GetPlayerRatingsAsync();
}
