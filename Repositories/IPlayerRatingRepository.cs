using FootballSimulationApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballSimulationApi.Repositories;

public interface IPlayerRatingRepository
{
    Task<IEnumerable<PlayerRatingDto>> GetPlayerRatingsAsync();
}
