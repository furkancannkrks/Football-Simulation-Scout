namespace FootballSimulationApi.DTOs;

public class PlayerScoutDto
{
    public string? Name { get; set; }
    public string? Team { get; set; }
    public string? Position { get; set; }
    
    public decimal? Appearances { get; set; }
    public decimal? MinutesPlayed { get; set; }
    
    public decimal? Goals { get; set; }
    public decimal? Assists { get; set; }
    public decimal? Xg { get; set; }
    public decimal? KeyPasses { get; set; }
    public decimal? AccuratePasses { get; set; }
    
    public decimal? Tackles { get; set; }
    public decimal? Interceptions { get; set; }
    public decimal? Clearances { get; set; }
    public decimal? AerialsWon { get; set; }
    public decimal? DuelsWon { get; set; }
    
    public decimal? Saves { get; set; }
    
    public double Rating { get; set; }
}
