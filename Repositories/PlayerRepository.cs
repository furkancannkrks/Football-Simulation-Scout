using FootballSimulationApi.DTOs;
using FootballSimulationApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballSimulationApi.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _context;

    public PlayerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlayerSeasonStat>> GetAllPlayersAsync()
    {
        return await _context.PlayerSeasonStats.ToListAsync();
    }

    public async Task<List<PlayerBasicDto>> GetAllBasicPlayersAsync()
    {
        return await _context.PlayerSeasonStats
            .Where(p => p.PlayerName != null && p.TeamName != null)
            .Select(p => new PlayerBasicDto
            {
                Name = p.PlayerName,
                Team = p.TeamName,
                Position = p.Position
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<PlayerSeasonStat>> GetPlayersByScoutFilterAsync(ScoutFilterDto filter)
    {
        var query = _context.PlayerSeasonStats.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(p => p.PlayerName != null && p.PlayerName.ToLower().Contains(filter.Name.ToLower()));

        if (filter.MinAppearances.HasValue)
            query = query.Where(p => p.Appearances >= filter.MinAppearances.Value);
            
        if (!string.IsNullOrEmpty(filter.Position))
            query = query.Where(p => p.Position == filter.Position);
            
        if (filter.MinGoals.HasValue)
            query = query.Where(p => p.Goals >= filter.MinGoals.Value);
            
        if (filter.MinXg.HasValue)
            query = query.Where(p => p.Xg >= (decimal)filter.MinXg.Value);
            
        if (filter.MinAssists.HasValue)
            query = query.Where(p => p.Assists >= filter.MinAssists.Value);
            
        if (filter.MinKeyPasses.HasValue)
            query = query.Where(p => p.KeyPasses >= filter.MinKeyPasses.Value);
            
        if (filter.MinAccuratePasses.HasValue)
            query = query.Where(p => p.AccuratePasses >= filter.MinAccuratePasses.Value);
            
        if (filter.MinTackles.HasValue)
            query = query.Where(p => p.Tackles >= filter.MinTackles.Value);
            
        if (filter.MinInterceptions.HasValue)
            query = query.Where(p => p.Interceptions >= filter.MinInterceptions.Value);
            
        if (filter.MinClearances.HasValue)
            query = query.Where(p => p.Clearances >= filter.MinClearances.Value);
            
        if (filter.MinAerialsWon.HasValue)
            query = query.Where(p => p.AerialsWon >= filter.MinAerialsWon.Value);
            
        if (filter.MinDuelsWon.HasValue)
            query = query.Where(p => p.DuelsWon >= filter.MinDuelsWon.Value);

        if (filter.MinMinutesPlayed.HasValue)
            query = query.Where(p => p.MinutesPlayed >= filter.MinMinutesPlayed.Value);
            
        if (filter.MinSaves.HasValue)
            query = query.Where(p => p.Saves >= filter.MinSaves.Value);

        return await query.ToListAsync();
    }

    public async Task<List<PlayerRatingDto>> GetTopRatedPlayersAsync(int limit)
    {
        return await _context.PlayerRatingDtos.FromSqlInterpolated($@"
            WITH Player_Raw_Scores AS (
                SELECT 
                    player_name AS ""Name"", 
                    team_name AS ""Team"", 
                    appearances,
                    ((xg * 2.5) + (key_passes * 0.8) + ((tackles + interceptions + duels_won) * 0.15)) AS raw_score
                FROM player_season_stats
                WHERE team_name IS NOT NULL AND appearances >= 5
            ),
            Per_Match_Scores AS (
                SELECT 
                    ""Name"", 
                    ""Team"", 
                    (raw_score / NULLIF(appearances, 0)) AS match_rating_score
                FROM Player_Raw_Scores
            )
            SELECT 
                ""Name"", 
                ""Team"", 
                ROUND(CAST((PERCENT_RANK() OVER (ORDER BY match_rating_score ASC)) * 9.0 + 1.0 AS NUMERIC), 1) AS ""Rating""
            FROM Per_Match_Scores
            WHERE match_rating_score IS NOT NULL
            ORDER BY ""Rating"" DESC
            LIMIT {limit}
        ").ToListAsync();
    }

    public async Task<PlayerSeasonStat?> GetPlayerByIdAsync(int id)
    {
        return await _context.PlayerSeasonStats.FindAsync(id);
    }

    public async Task InsertPlayerAsync(PlayerSeasonStat player)
    {
        await _context.PlayerSeasonStats.AddAsync(player);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePlayerAsync(PlayerSeasonStat player)
    {
        _context.PlayerSeasonStats.Update(player);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePlayerAsync(int id)
    {
        var player = await _context.PlayerSeasonStats.FindAsync(id);
        if (player != null)
        {
            _context.PlayerSeasonStats.Remove(player);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<PlayerSeasonStat>> GetPlayersByNamesAsync(List<string> playerNames)
    {
        // Case insensitive search
        var lowerNames = playerNames.Select(n => n.ToLower()).ToList();
        
        return await _context.PlayerSeasonStats
            .Where(p => p.PlayerName != null && lowerNames.Contains(p.PlayerName.ToLower()))
            .ToListAsync();
    }

    public async Task<List<TeamStatsDto>> GetTeamStatsAsync()
    {
        var teamSeasonStats = await _context.PlayerSeasonStats
            .Where(p => p.TeamName != null)
            .GroupBy(p => p.TeamName)
            .Select(g => new
            {
                TeamName = g.Key,
                TotalGoals = (int)g.Sum(p => p.Goals ?? 0),
                TotalAssists = (int)g.Sum(p => p.Assists ?? 0),
                PlayerCount = g.Count()
            })
            .ToListAsync();

        var teamMatchXgs = await _context.TeamMatchStats
            .Where(t => t.TeamName != null)
            .GroupBy(t => t.TeamName)
            .Select(g => new
            {
                TeamName = g.Key,
                AvgXg = g.Average(t => t.Xg ?? 0)
            })
            .ToDictionaryAsync(t => t.TeamName!, t => t.AvgXg);

        return teamSeasonStats
            .Select(t => new TeamStatsDto
            {
                Team = t.TeamName,
                TotalGoals = t.TotalGoals,
                TotalAssists = t.TotalAssists,
                PlayerCount = t.PlayerCount,
                AvgXg = teamMatchXgs.TryGetValue(t.TeamName!, out var xg) ? Math.Round((double)xg, 2) : 0.0
            })
            .OrderByDescending(t => t.AvgXg)
            .ToList();
    }

    public async Task<List<TeamRosterDto>> GetPlayersByTeamAsync(string teamName)
    {
        return await _context.PlayerSeasonStats
            .Where(p => p.TeamName == teamName)
            .Select(p => new TeamRosterDto
            {
                StatId = p.StatId,
                Name = p.PlayerName,
                Position = p.Position,
                Appearances = (int)(p.Appearances ?? 0m),
                Goals = (int)(p.Goals ?? 0m),
                Assists = (int)(p.Assists ?? 0m),
                Xg = p.Xg ?? 0m,
                KeyPasses = (int)(p.KeyPasses ?? 0m),
                Tackles = (int)(p.Tackles ?? 0m),
                Interceptions = (int)(p.Interceptions ?? 0m),
                AerialsWon = (int)(p.AerialsWon ?? 0m),
                MinutesPlayed = (int)(p.MinutesPlayed ?? 0m)
            })
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
