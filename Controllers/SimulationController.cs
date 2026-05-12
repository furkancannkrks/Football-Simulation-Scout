using FootballSimulationApi.DTOs;
using FootballSimulationApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FootballSimulationApi.Controllers;

[ApiController]
[Route("api/simulation")]
public class SimulationController : ControllerBase
{
    private readonly ISimulationService _simulationService;

    public SimulationController(ISimulationService simulationService)
    {
        _simulationService = simulationService;
    }

    [HttpPost("match")]
    public async Task<ActionResult<MatchSimulationResultDto>> SimulateMatch([FromBody] MatchSimulationRequestDto request)
    {
        if (request == null || string.IsNullOrEmpty(request.HomeTeam) || string.IsNullOrEmpty(request.AwayTeam))
            return BadRequest("Invalid simulation request.");

        var result = await _simulationService.SimulateMatchAsync(request);
        return Ok(result);
    }
}
