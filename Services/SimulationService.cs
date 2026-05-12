using FootballSimulationApi.DTOs;
using FootballSimulationApi.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FootballSimulationApi.Services;

public class SimulationService : ISimulationService
{
    private readonly IPlayerRepository _playerRepository;

    public SimulationService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<MatchSimulationResultDto> SimulateMatchAsync(MatchSimulationRequestDto request)
    {
        var homePlayers = await _playerRepository.GetPlayersByNamesAsync(request.HomeSquad);
        var awayPlayers = await _playerRepository.GetPlayersByNamesAsync(request.AwaySquad);

        // Home team calculations
        double homeTotXg = (double)homePlayers.Sum(p => p.Xg ?? 0m);
        double homeTotKp = (double)homePlayers.Sum(p => p.KeyPasses ?? 0m);
        double homeTotTackles = (double)homePlayers.Sum(p => p.Tackles ?? 0m);
        double homeTotInter = (double)homePlayers.Sum(p => p.Interceptions ?? 0m);

        double hAtk = (homeTotXg * 1.5) + (homeTotKp * 0.3);
        double hDef = (homeTotTackles * 0.2) + (homeTotInter * 0.2);

        // Away team calculations
        double awayTotXg = (double)awayPlayers.Sum(p => p.Xg ?? 0m);
        double awayTotKp = (double)awayPlayers.Sum(p => p.KeyPasses ?? 0m);
        double awayTotTackles = (double)awayPlayers.Sum(p => p.Tackles ?? 0m);
        double awayTotInter = (double)awayPlayers.Sum(p => p.Interceptions ?? 0m);

        double aAtk = (awayTotXg * 1.5) + (awayTotKp * 0.3);
        double aDef = (awayTotTackles * 0.2) + (awayTotInter * 0.2);

        // Expected goals calculation
        double homeExpGoals = (hAtk / Math.Max(aDef, 1.0)) + 0.3;
        double awayExpGoals = (aAtk / Math.Max(hDef, 1.0));
        
        // Poisson calculation for scores up to 5
        double bestProb = 0;
        int bestHomeScore = 0;
        int bestAwayScore = 0;
        
        double pWinHome = 0;
        double pWinAway = 0;
        double pDraw = 0;
        
        double pOver15 = 0;
        double pOver25 = 0;
        double pOver35 = 0;
        
        double pCleanSheetHome = 0; // Away scores 0
        double pCleanSheetAway = 0; // Home scores 0
        
        double pBttsYes = 0;

        for (int h = 0; h <= 5; h++)
        {
            for (int a = 0; a <= 5; a++)
            {
                double prob = Poisson(h, homeExpGoals) * Poisson(a, awayExpGoals);
                
                if (prob > bestProb)
                {
                    bestProb = prob;
                    bestHomeScore = h;
                    bestAwayScore = a;
                }
                
                if (h > a) pWinHome += prob;
                else if (a > h) pWinAway += prob;
                else pDraw += prob;
                
                if (h + a > 1.5) pOver15 += prob;
                if (h + a > 2.5) pOver25 += prob;
                if (h + a > 3.5) pOver35 += prob;
                
                if (a == 0) pCleanSheetHome += prob;
                if (h == 0) pCleanSheetAway += prob;
                
                if (h > 0 && a > 0) pBttsYes += prob;
            }
        }
        
        // Normalize primary outcomes due to capping at 5
        double sum = pWinHome + pWinAway + pDraw;
        pWinHome /= sum;
        pWinAway /= sum;
        pDraw /= sum;

        int winHomePct = Math.Clamp((int)(pWinHome * 100), 0, 100);
        int winAwayPct = Math.Clamp((int)(pWinAway * 100), 0, 100);
        int drawPct = 100 - (winHomePct + winAwayPct);
        
        // Corners calculation (based on total attack)
        double totalAttack = hAtk + aAtk;
        int cornerOver85Pct = Math.Clamp((int)((totalAttack / 30.0) * 100), 10, 95);

        return new MatchSimulationResultDto
        {
            WinHomePct = winHomePct,
            DrawPct = drawPct,
            WinAwayPct = winAwayPct,
            
            Over15Pct = Math.Clamp((int)(pOver15 * 100), 1, 99),
            Under15Pct = 100 - Math.Clamp((int)(pOver15 * 100), 1, 99),
            
            Over25Pct = Math.Clamp((int)(pOver25 * 100), 1, 99),
            Under25Pct = 100 - Math.Clamp((int)(pOver25 * 100), 1, 99),
            
            Over35Pct = Math.Clamp((int)(pOver35 * 100), 1, 99),
            Under35Pct = 100 - Math.Clamp((int)(pOver35 * 100), 1, 99),
            
            BttsYesPct = Math.Clamp((int)(pBttsYes * 100), 1, 99),
            BttsNoPct = 100 - Math.Clamp((int)(pBttsYes * 100), 1, 99),
            
            HomeExpGoals = Math.Round(homeExpGoals, 2),
            AwayExpGoals = Math.Round(awayExpGoals, 2),
            
            MostLikelyHomeScore = bestHomeScore,
            MostLikelyAwayScore = bestAwayScore,
            MostLikelyScoreline = $"{bestHomeScore}-{bestAwayScore}",
            CorrectScorePct = Math.Clamp((int)(bestProb * 100), 1, 100),
            
            CleanSheetHomePct = Math.Clamp((int)(pCleanSheetHome * 100), 1, 99),
            CleanSheetAwayPct = Math.Clamp((int)(pCleanSheetAway * 100), 1, 99),
            
            CornerOver85Pct = cornerOver85Pct,
            
            DoubleChanceHome = winHomePct + drawPct,
            DoubleChanceAway = winAwayPct + drawPct
        };
    }

    private double Poisson(int k, double lambda)
    {
        return Math.Exp(-lambda) * Math.Pow(lambda, k) / Factorial(k);
    }

    private int Factorial(int n)
    {
        if (n <= 1) return 1;
        int result = 1;
        for (int i = 2; i <= n; i++) result *= i;
        return result;
    }
}
