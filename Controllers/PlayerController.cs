using FootballSimulationApi.DTOs;
using FootballSimulationApi.Models;
using FootballSimulationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FootballSimulationApi.Controllers;

[ApiController]
[Route("api/players")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet("ratings-legacy")]
    public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetTopRatings([FromQuery] int limit = 20)
    {
        var players = await _playerService.GetTopRatingsAsync(limit);
        return Ok(players);
    }

    [HttpGet("all-basic")]
    public async Task<ActionResult<List<PlayerBasicDto>>> GetAllBasicPlayers()
    {
        var players = await _playerService.GetAllBasicPlayersAsync();
        return Ok(players);
    }

    [HttpGet("scout")]
    public async Task<ActionResult<IEnumerable<PlayerScoutDto>>> GetScoutedPlayers([FromQuery] ScoutFilterDto filter)
    {
        var players = await _playerService.GetScoutedPlayersAsync(filter);
        return Ok(players);
    }

    [HttpPost]
    public async Task<ActionResult> InsertPlayer([FromBody] PlayerSeasonStat player)
    {
        await _playerService.InsertPlayerAsync(player);
        return CreatedAtAction(nameof(GetPlayerById), new { id = player.StatId }, player);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdatePlayer(int id, [FromBody] PlayerSeasonStat player)
    {
        if (id != player.StatId)
            return BadRequest("ID mismatch in the request.");

        await _playerService.UpdatePlayerAsync(player);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePlayer(int id)
    {
        await _playerService.DeletePlayerAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerSeasonStat>> GetPlayerById(int id)
    {
        var player = await _playerService.GetPlayerByIdAsync(id);
        if (player == null)
            return NotFound();

        return Ok(player);
    }

    [HttpGet("team-stats")]
    public async Task<ActionResult<List<TeamStatsDto>>> GetTeamStats()
    {
        var stats = await _playerService.GetTeamStatsAsync();
        return Ok(stats);
    }

    [HttpGet("by-team")]
    public async Task<ActionResult<IEnumerable<TeamRosterDto>>> GetPlayersByTeam([FromQuery] string teamName)
    {
        if (string.IsNullOrEmpty(teamName))
            return BadRequest("Team name is required.");

        var roster = await _playerService.GetPlayersByTeamAsync(teamName);
        return Ok(roster);
    }
}
