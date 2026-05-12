namespace FootballSimulationApi.DTOs;

public class TeamRosterDto
{
    public int StatId { get; set; }
    public string? Name { get; set; }
    public string? Position { get; set; }
    public int Appearances { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public decimal Xg { get; set; }
    public int KeyPasses { get; set; }
    public int Tackles { get; set; }
    public int Interceptions { get; set; }
    public int AerialsWon { get; set; }
    public int MinutesPlayed { get; set; }
}
