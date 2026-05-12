using Dapper;
using FootballSimulationApi.DTOs;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballSimulationApi.Repositories;

public class PlayerRatingRepository : IPlayerRatingRepository
{
    private readonly IConfiguration _configuration;

    public PlayerRatingRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IEnumerable<PlayerRatingDto>> GetPlayerRatingsAsync()
    {
        try
        {
            const string sql = @"
WITH Per90 AS (
    SELECT
        player_name, team_name, position, appearances,
        minutes_played AS mins,
        NULLIF(minutes_played / 90.0, 0) AS p90,

        -- Attacking
        COALESCE(goals, 0)               / NULLIF(minutes_played/90.0,0) AS goals_p90,
        COALESCE(shots_on_target, 0)     / NULLIF(minutes_played/90.0,0) AS sot_p90,
        COALESCE(xg, 0)                  / NULLIF(minutes_played/90.0,0) AS xg_p90,
        COALESCE(npxg, 0)                / NULLIF(minutes_played/90.0,0) AS npxg_p90,

        -- Playmaking
        COALESCE(assists, 0)             / NULLIF(minutes_played/90.0,0) AS ast_p90,
        COALESCE(xa, 0)                  / NULLIF(minutes_played/90.0,0) AS xa_p90,
        COALESCE(key_passes, 0)          / NULLIF(minutes_played/90.0,0) AS kp_p90,
        COALESCE(big_chances_created, 0) / NULLIF(minutes_played/90.0,0) AS bcc_p90,
        COALESCE(accurate_passes, 0)     / NULLIF(minutes_played/90.0,0) AS acc_p90,
        COALESCE(accurate_long_balls, 0) / NULLIF(minutes_played/90.0,0) AS alb_p90,
        COALESCE(crosses, 0)             / NULLIF(minutes_played/90.0,0) AS cross_p90,

        -- Defending
        COALESCE(tackles, 0)             / NULLIF(minutes_played/90.0,0) AS tkl_p90,
        COALESCE(interceptions, 0)       / NULLIF(minutes_played/90.0,0) AS int_p90,
        COALESCE(clearances, 0)          / NULLIF(minutes_played/90.0,0) AS clr_p90,
        COALESCE(blocks, 0)              / NULLIF(minutes_played/90.0,0) AS blk_p90,
        COALESCE(aerials_won, 0)         / NULLIF(minutes_played/90.0,0) AS aw_p90,

        -- Physical / Duels
        COALESCE(duels_won, 0)           / NULLIF(minutes_played/90.0,0) AS dw_p90,
        COALESCE(duels_lost, 0)          / NULLIF(minutes_played/90.0,0) AS dl_p90,
        COALESCE(dispossessed, 0)        / NULLIF(minutes_played/90.0,0) AS disp_p90,

        -- GK
        COALESCE(saves, 0)               / NULLIF(minutes_played/90.0,0) AS saves_p90

    FROM player_season_stats
    WHERE minutes_played >= 450
      AND appearances >= 5
),

Ranked AS (
    SELECT *,
        -- Attacking ranks
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY goals_p90)  AS r_goals,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY xg_p90)     AS r_xg,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY npxg_p90)   AS r_npxg,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY sot_p90)    AS r_sot,

        -- Playmaking ranks
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY ast_p90)    AS r_ast,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY xa_p90)     AS r_xa,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY kp_p90)     AS r_kp,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY bcc_p90)    AS r_bcc,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY acc_p90)    AS r_acc,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY alb_p90)    AS r_alb,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY cross_p90)  AS r_cross,

        -- Defending ranks
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY tkl_p90)    AS r_tkl,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY int_p90)    AS r_int,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY clr_p90)    AS r_clr,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY blk_p90)    AS r_blk,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY aw_p90)     AS r_aw,

        -- Duels / Physical ranks
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY dw_p90)     AS r_dw,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY dl_p90 DESC) AS r_dl_inv,
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY disp_p90 DESC) AS r_disp_inv,

        -- GK ranks
        PERCENT_RANK() OVER (PARTITION BY position ORDER BY saves_p90)  AS r_saves

    FROM Per90
),

WeightedScore AS (
    SELECT
        player_name, team_name, position, mins, appearances,
        CASE
            WHEN position = 'F' THEN
                (r_goals  * 0.22) +
                (r_xg     * 0.15) +
                (r_npxg   * 0.10) +
                (r_sot    * 0.08) +
                (r_ast    * 0.10) +
                (r_xa     * 0.08) +
                (r_kp     * 0.06) +
                (r_bcc    * 0.07) +
                (r_dw     * 0.06) +
                (r_dl_inv * 0.04) +
                (r_disp_inv * 0.04)

            WHEN position = 'M' THEN
                (r_kp     * 0.14) +
                (r_ast    * 0.12) +
                (r_xa     * 0.10) +
                (r_bcc    * 0.08) +
                (r_acc    * 0.08) +
                (r_goals  * 0.08) +
                (r_tkl    * 0.10) +
                (r_int    * 0.08) +
                (r_dw     * 0.08) +
                (r_dl_inv * 0.07) +
                (r_disp_inv * 0.07)

            WHEN position = 'D' THEN
                (r_tkl    * 0.18) +
                (r_int    * 0.18) +
                (r_clr    * 0.12) +
                (r_blk    * 0.08) +
                (r_aw     * 0.10) +
                (r_dw     * 0.08) +
                (r_dl_inv * 0.08) +
                (r_acc    * 0.06) +
                (r_alb    * 0.06) +
                (r_disp_inv * 0.06)

            WHEN position = 'G' THEN
                (r_saves  * 0.60) +
                (r_acc    * 0.15) +
                (r_alb    * 0.15) +
                (r_clr    * 0.10)

            ELSE
                (r_goals + r_ast + r_kp + r_tkl + r_int + r_dw) / 6.0
        END AS weighted_score

    FROM Ranked
),

FinalRank AS (
    SELECT *,
        PERCENT_RANK() OVER (ORDER BY weighted_score ASC) AS final_pct
    FROM WeightedScore
)

SELECT
    player_name                                              AS oyuncu,
    player_name                                              AS name,
    team_name                                                AS team,
    position                                                 AS pos,
    appearances                                              AS mac,
    mins                                                     AS dakika,
    ROUND(CAST((final_pct * 9.0) + 1.0 AS numeric), 2)       AS rating
FROM FinalRank
ORDER BY rating DESC
LIMIT 20;
";

            await using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            return await connection.QueryAsync<PlayerRatingDto>(sql);
        }
        catch (Exception ex)
        {
            return new List<PlayerRatingDto> { new() { Oyuncu = ex.Message, Name = ex.Message } };
        }
    }
}
