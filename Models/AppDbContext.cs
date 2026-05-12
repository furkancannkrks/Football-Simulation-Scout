using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using FootballSimulationApi.DTOs;

namespace FootballSimulationApi.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PlayerSeasonStat> PlayerSeasonStats { get; set; }

    public virtual DbSet<TeamMatchStat> TeamMatchStats { get; set; }
    public virtual DbSet<PlayerRatingDto> PlayerRatingDtos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerRatingDto>().HasNoKey();

        modelBuilder.Entity<PlayerSeasonStat>(entity =>
        {
            entity.HasKey(e => e.StatId).HasName("player_season_stats_pkey");

            entity.ToTable("player_season_stats");

            entity.Property(e => e.StatId).HasColumnName("stat_id");
            entity.Property(e => e.AccurateLongBalls).HasColumnName("accurate_long_balls");
            entity.Property(e => e.AccuratePasses).HasColumnName("accurate_passes");
            entity.Property(e => e.AerialsWon).HasColumnName("aerials_won");
            entity.Property(e => e.Appearances).HasColumnName("appearances");
            entity.Property(e => e.Assists).HasColumnName("assists");
            entity.Property(e => e.BigChancesCreated).HasColumnName("big_chances_created");
            entity.Property(e => e.Blocks).HasColumnName("blocks");
            entity.Property(e => e.Clearances).HasColumnName("clearances");
            entity.Property(e => e.Crosses).HasColumnName("crosses");
            entity.Property(e => e.Dispossessed).HasColumnName("dispossessed");
            entity.Property(e => e.DuelsLost).HasColumnName("duels_lost");
            entity.Property(e => e.DuelsWon).HasColumnName("duels_won");
            entity.Property(e => e.Goals).HasColumnName("goals");
            entity.Property(e => e.Interceptions).HasColumnName("interceptions");
            entity.Property(e => e.KeyPasses).HasColumnName("key_passes");
            entity.Property(e => e.LongBalls).HasColumnName("long_balls");
            entity.Property(e => e.MinutesPlayed).HasColumnName("minutes_played");
            entity.Property(e => e.Npxg).HasColumnName("npxg");
            entity.Property(e => e.Passes).HasColumnName("passes");
            entity.Property(e => e.PlayerName)
                .HasMaxLength(100)
                .HasColumnName("player_name");
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .HasColumnName("position");
            entity.Property(e => e.Saves).HasColumnName("saves");
            entity.Property(e => e.Shots).HasColumnName("shots");
            entity.Property(e => e.ShotsOnTarget).HasColumnName("shots_on_target");
            entity.Property(e => e.Tackles).HasColumnName("tackles");
            entity.Property(e => e.TeamName)
                .HasMaxLength(100)
                .HasColumnName("team_name");
            entity.Property(e => e.Xa).HasColumnName("xa");
            entity.Property(e => e.Xg).HasColumnName("xg");
        });

        modelBuilder.Entity<TeamMatchStat>(entity =>
        {
            entity.HasKey(e => e.MatchStatId).HasName("team_match_stats_pkey");

            entity.ToTable("team_match_stats");

            entity.Property(e => e.MatchStatId).HasColumnName("match_stat_id");
            entity.Property(e => e.BigChanceCreated).HasColumnName("big_chance_created");
            entity.Property(e => e.BigChanceMissed).HasColumnName("big_chance_missed");
            entity.Property(e => e.BigChanceScored).HasColumnName("big_chance_scored");
            entity.Property(e => e.Cards).HasColumnName("cards");
            entity.Property(e => e.Clearances).HasColumnName("clearances");
            entity.Property(e => e.Corners).HasColumnName("corners");
            entity.Property(e => e.Crosses).HasColumnName("crosses");
            entity.Property(e => e.Dispossessed).HasColumnName("dispossessed");
            entity.Property(e => e.ErrorsLeadToGoal).HasColumnName("errors_lead_to_goal");
            entity.Property(e => e.ErrorsLeadToShot).HasColumnName("errors_lead_to_shot");
            entity.Property(e => e.Fouls).HasColumnName("fouls");
            entity.Property(e => e.FreeKicks).HasColumnName("free_kicks");
            entity.Property(e => e.GoalKicks).HasColumnName("goal_kicks");
            entity.Property(e => e.GoalkeeperSaves).HasColumnName("goalkeeper_saves");
            entity.Property(e => e.HomeAway)
                .HasMaxLength(50)
                .HasColumnName("home_away");
            entity.Property(e => e.InterceptionWon).HasColumnName("interception_won");
            entity.Property(e => e.MatchScore)
                .HasMaxLength(20)
                .HasColumnName("match_score");
            entity.Property(e => e.Offsides).HasColumnName("offsides");
            entity.Property(e => e.OpponentGoals).HasColumnName("opponent_goals");
            entity.Property(e => e.OpponentName)
                .HasMaxLength(100)
                .HasColumnName("opponent_name");
            entity.Property(e => e.Passes).HasColumnName("passes");
            entity.Property(e => e.Possession).HasColumnName("possession");
            entity.Property(e => e.RedCards).HasColumnName("red_cards");
            entity.Property(e => e.ShotsInTheBox).HasColumnName("shots_in_the_box");
            entity.Property(e => e.ShotsOnTarget).HasColumnName("shots_on_target");
            entity.Property(e => e.ShotsOutsideTheBox).HasColumnName("shots_outside_the_box");
            entity.Property(e => e.Tackles).HasColumnName("tackles");
            entity.Property(e => e.TeamGoals).HasColumnName("team_goals");
            entity.Property(e => e.TeamName)
                .HasMaxLength(100)
                .HasColumnName("team_name");
            entity.Property(e => e.ThrowIns).HasColumnName("throw_ins");
            entity.Property(e => e.TotalShots).HasColumnName("total_shots");
            entity.Property(e => e.TouchesInOppBox).HasColumnName("touches_in_opp_box");
            entity.Property(e => e.Xg).HasColumnName("xg");
            entity.Property(e => e.YellowCards).HasColumnName("yellow_cards");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
