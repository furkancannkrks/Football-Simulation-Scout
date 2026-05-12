using System.Collections.Generic;

namespace FootballSimulationApi.DTOs;

public class MatchSimulationRequestDto
{
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
    public List<string> HomeSquad { get; set; } = new List<string>();
    public List<string> AwaySquad { get; set; } = new List<string>();
}
