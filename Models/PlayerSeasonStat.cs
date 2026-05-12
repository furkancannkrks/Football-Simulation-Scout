using System;
using System.Collections.Generic;

namespace FootballSimulationApi.Models;

public partial class PlayerSeasonStat
{
    public int StatId { get; set; }

    public string? PlayerName { get; set; }

    public string? Position { get; set; }

    public decimal? Appearances { get; set; }

    public decimal? Passes { get; set; }

    public decimal? AccuratePasses { get; set; }

    public decimal? LongBalls { get; set; }

    public decimal? AccurateLongBalls { get; set; }

    public decimal? Assists { get; set; }

    public decimal? Crosses { get; set; }

    public decimal? BigChancesCreated { get; set; }

    public decimal? Shots { get; set; }

    public decimal? ShotsOnTarget { get; set; }

    public decimal? Goals { get; set; }

    public decimal? Tackles { get; set; }

    public decimal? Clearances { get; set; }

    public decimal? Interceptions { get; set; }

    public decimal? AerialsWon { get; set; }

    public decimal? Blocks { get; set; }

    public decimal? MinutesPlayed { get; set; }

    public decimal? Xg { get; set; }

    public decimal? Npxg { get; set; }

    public decimal? KeyPasses { get; set; }

    public decimal? Xa { get; set; }

    public decimal? Dispossessed { get; set; }

    public decimal? Saves { get; set; }

    public decimal? DuelsWon { get; set; }

    public decimal? DuelsLost { get; set; }

    public string? TeamName { get; set; }
}
