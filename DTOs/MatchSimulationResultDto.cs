namespace FootballSimulationApi.DTOs;

public class MatchSimulationResultDto
{
    public int WinHomePct { get; set; }
    public int DrawPct { get; set; }
    public int WinAwayPct { get; set; }
    public int Over25Pct { get; set; }
    public int Under25Pct { get; set; }
    public int BttsYesPct { get; set; }
    public int BttsNoPct { get; set; }
    public double HomeExpGoals { get; set; }
    public double AwayExpGoals { get; set; }
    
    // New fields
    public int MostLikelyHomeScore { get; set; }
    public int MostLikelyAwayScore { get; set; }
    public string? MostLikelyScoreline { get; set; }
    public int CorrectScorePct { get; set; }
    public int Over35Pct { get; set; }
    public int Under35Pct { get; set; }
    public int Over15Pct { get; set; }
    public int Under15Pct { get; set; }
    public int CleanSheetHomePct { get; set; }
    public int CleanSheetAwayPct { get; set; }
    public int CornerOver85Pct { get; set; }
    public int DoubleChanceHome { get; set; }
    public int DoubleChanceAway { get; set; }
}
