using FootballSimulationApi.DTOs;
using System.Threading.Tasks;

namespace FootballSimulationApi.Services;

public interface ISimulationService
{
    Task<MatchSimulationResultDto> SimulateMatchAsync(MatchSimulationRequestDto request);
}
