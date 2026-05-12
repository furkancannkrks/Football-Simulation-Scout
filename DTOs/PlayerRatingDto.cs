namespace FootballSimulationApi.DTOs;

public class PlayerRatingDto
{
    // New properties for Dapper Player Ratings query
    public string Oyuncu { get; set; } = null!;
    public string Pos { get; set; } = null!;
    public int Mac { get; set; }
    public int Dakika { get; set; }
    public decimal Rating { get; set; }

    // Legacy properties retained for backward compatibility with existing EF Core queries
    public string Name { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
}
