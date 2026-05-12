namespace FootballSimulationApi.DTOs;

public class PlayerRatingDto
{
    public string Name { get; set; } = null!;
    public string Team { get; set; } = null!;
    public decimal Rating { get; set; }
}
