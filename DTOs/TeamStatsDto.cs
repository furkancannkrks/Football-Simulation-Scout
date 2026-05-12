namespace FootballSimulationApi.DTOs;

public class TeamStatsDto
{
    public string? Team { get; set; }
    public int TotalGoals { get; set; }
    public int TotalAssists { get; set; }
    public double AvgXg { get; set; }
    public int PlayerCount { get; set; }
}
