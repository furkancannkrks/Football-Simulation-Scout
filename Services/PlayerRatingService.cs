using FootballSimulationApi.DTOs;
using FootballSimulationApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballSimulationApi.Services;

public class PlayerRatingService : IPlayerRatingService
{
    private readonly IPlayerRatingRepository _playerRatingRepository;

    public PlayerRatingService(IPlayerRatingRepository playerRatingRepository)
    {
        _playerRatingRepository = playerRatingRepository;
    }

    public async Task<IEnumerable<PlayerRatingDto>> GetPlayerRatingsAsync()
    {
        return await _playerRatingRepository.GetPlayerRatingsAsync();
    }
}
