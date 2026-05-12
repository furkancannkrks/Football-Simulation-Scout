using Dapper;
using FootballSimulationApi.DTOs;
using FootballSimulationApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballSimulationApi.Controllers;

[ApiController]
[Route("api/players")]
public class PlayerRatingController : ControllerBase
{
    private readonly IPlayerRatingService _playerRatingService;
    private readonly IConfiguration _configuration;

    public PlayerRatingController(IPlayerRatingService playerRatingService, IConfiguration configuration)
    {
        _playerRatingService = playerRatingService;
        _configuration = configuration;
    }

    [HttpGet("ratings")]
    public async Task<ActionResult<IEnumerable<PlayerRatingDto>>> GetPlayerRatings()
    {
        var ratings = await _playerRatingService.GetPlayerRatingsAsync();
        return Ok(ratings);
    }

    [HttpGet("ratings/debug")]
    public async Task<IActionResult> Debug()
    {
        await using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        var result = await conn.QueryAsync<dynamic>(
            "SELECT column_name FROM information_schema.columns WHERE table_name = 'player_season_stats'"
        );
        return Ok(result);
    }
}
