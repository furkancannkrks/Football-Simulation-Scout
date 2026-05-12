using System;
using System.Collections.Generic;

namespace FootballSimulationApi.Models;

public partial class TeamMatchStat
{
    public int MatchStatId { get; set; }

    public string? TeamName { get; set; }

    public string? OpponentName { get; set; }

    public string? HomeAway { get; set; }

    public string? MatchScore { get; set; }

    public decimal? TeamGoals { get; set; }

    public decimal? OpponentGoals { get; set; }

    public decimal? Corners { get; set; }

    public decimal? Cards { get; set; }

    public decimal? Crosses { get; set; }

    public decimal? BigChanceCreated { get; set; }

    public decimal? BigChanceMissed { get; set; }

    public decimal? BigChanceScored { get; set; }

    public decimal? Xg { get; set; }

    public decimal? ShotsOnTarget { get; set; }

    public decimal? ShotsInTheBox { get; set; }

    public decimal? TotalShots { get; set; }

    public decimal? ShotsOutsideTheBox { get; set; }

    public decimal? Clearances { get; set; }

    public decimal? Dispossessed { get; set; }

    public decimal? ErrorsLeadToGoal { get; set; }

    public decimal? ErrorsLeadToShot { get; set; }

    public decimal? Fouls { get; set; }

    public decimal? GoalkeeperSaves { get; set; }

    public decimal? InterceptionWon { get; set; }

    public decimal? Tackles { get; set; }

    public decimal? FreeKicks { get; set; }

    public decimal? GoalKicks { get; set; }

    public decimal? ThrowIns { get; set; }

    public decimal? Possession { get; set; }

    public decimal? Offsides { get; set; }

    public decimal? Passes { get; set; }

    public decimal? TouchesInOppBox { get; set; }

    public decimal? RedCards { get; set; }

    public decimal? YellowCards { get; set; }
}
