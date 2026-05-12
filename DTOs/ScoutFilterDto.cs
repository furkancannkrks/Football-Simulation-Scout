namespace FootballSimulationApi.DTOs;

public class ScoutFilterDto
{
    public string? Name { get; set; }
    public int? MinAppearances { get; set; }
    public string? Position { get; set; }
    public int? MinGoals { get; set; }
    public double? MinXg { get; set; }
    public int? MinAssists { get; set; }
    public int? MinKeyPasses { get; set; }
    public int? MinAccuratePasses { get; set; }
    public int? MinTackles { get; set; }
    public int? MinInterceptions { get; set; }
    public int? MinClearances { get; set; }
    public int? MinAerialsWon { get; set; }
    public int? MinDuelsWon { get; set; }
    
    // New Filters (Excluding Age)
    public int? MinMinutesPlayed { get; set; }
    public int? MinSaves { get; set; }
}
