using MatchEntities;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    /// <summary>
    /// Metadata about a match of csgo.
    /// 
    /// Tightly coupled to MatchEntities.MatchStats, so be aware of redundancy to MatchDb.
    /// </summary>
    public class MatchEntity
    {
        /// <summary>
        /// Parameterless constructor required by EF Core.
        /// </summary>
        public MatchEntity()
        {

        }

        public MatchEntity(MatchStats matchStats)
        {
            MatchId = matchStats.MatchId;
            MatchDate = matchStats.MatchDate;
            Map = matchStats.Map;
            WinnerTeam = matchStats.WinnerTeam;
            TeamStats = matchStats.TeamStats;
            Source = matchStats.Source;
            GameType = matchStats.GameType;
            AvgRank = matchStats.AvgRank;
        }

        public long MatchId { get; set; }
        public DateTime MatchDate { get; set; }
        public string Map { get; set; }
        public StartingFaction WinnerTeam { get; set; }

        #region TeamMatchStats
        // team 1 is the team that starts as terrorists first
        // team 2 is CtStarter
        private short Score1 { get; set; }
        private short Score2 { get; set; }
        private short RealScore1 { get; set; }
        private short RealScore2 { get; set; }
        private short NumRoundsT1 { get; set; }
        private short NumRoundsCt1 { get; set; }
        private short NumRoundsT2 { get; set; }
        private short NumRoundsCt2 { get; set; }
        private short BombPlants1 { get; set; }
        private short BombPlants2 { get; set; }
        private short BombExplodes1 { get; set; }
        private short BombExplodes2 { get; set; }
        private short BombDefuses1 { get; set; }
        private short BombDefuses2 { get; set; }
        private int MoneyEarned1 { get; set; }
        private int MoneyEarned2 { get; set; }
        private int MoneySpent1 { get; set; }
        private int MoneySpent2 { get; set; }

        public Dictionary<StartingFaction, TeamMatchStats> TeamStats
        {
            get
            {
                return new Dictionary<StartingFaction, TeamMatchStats>
                {
                    {
                        StartingFaction.TerroristStarter,
                        new TeamMatchStats
                        {
                            Score = Score1,
                            RealScore = RealScore1,
                            BombDefuses = BombDefuses1,
                            BombExplodes = BombExplodes1,
                            BombPlants = BombPlants1,
                            MoneyEarned = MoneyEarned1,
                            MoneySpent = MoneySpent1,
                            NumRoundsCt = NumRoundsCt1,
                            NumRoundsT = NumRoundsT1,
                        }
                    },
                    {
                        StartingFaction.CtStarter,
                        new TeamMatchStats
                        {
                            Score = Score2,
                            RealScore = RealScore2,
                            BombDefuses = BombDefuses2,
                            BombExplodes = BombExplodes2,
                            BombPlants = BombPlants2,
                            MoneyEarned = MoneyEarned2,
                            MoneySpent = MoneySpent2,
                            NumRoundsCt = NumRoundsCt2,
                            NumRoundsT = NumRoundsT2,
                        }
                    }
                };
            }
            set
            {
                if (value.TryGetValue(StartingFaction.TerroristStarter, out var terroristStarterTeamStats))
                {
                    Score1 = terroristStarterTeamStats.Score;
                    RealScore1 = terroristStarterTeamStats.RealScore;
                    BombDefuses1 = terroristStarterTeamStats.BombDefuses;
                    BombExplodes1 = terroristStarterTeamStats.BombExplodes;
                    BombPlants1 = terroristStarterTeamStats.BombPlants;
                    MoneyEarned1 = terroristStarterTeamStats.MoneyEarned;
                    MoneySpent1 = terroristStarterTeamStats.MoneySpent;
                    NumRoundsCt1 = terroristStarterTeamStats.NumRoundsCt;
                    NumRoundsT1 = terroristStarterTeamStats.NumRoundsT;
                }
                if (value.TryGetValue(StartingFaction.CtStarter, out var ctStarterTeamStats))
                {
                    Score2 = ctStarterTeamStats.Score;
                    RealScore2 = terroristStarterTeamStats.RealScore;
                    BombDefuses2 = ctStarterTeamStats.BombDefuses;
                    BombExplodes2 = ctStarterTeamStats.BombExplodes;
                    BombPlants2 = ctStarterTeamStats.BombPlants;
                    MoneyEarned2 = ctStarterTeamStats.MoneyEarned;
                    MoneySpent2 = ctStarterTeamStats.MoneySpent;
                    NumRoundsCt2 = ctStarterTeamStats.NumRoundsCt;
                    NumRoundsT2 = ctStarterTeamStats.NumRoundsT;
                }

                if (value.ContainsKey(StartingFaction.Spectate))
                {
                    throw new KeyNotFoundException("Tried to assign a MatchStats.TeamStats value for Spectators team, but spectators shouldn't have one such entry.");
                }
            }
        }

        #endregion

        public Source Source { get; set; }
        public GameType GameType { get; set; }
        public float? AvgRank { get; set; }
    }
}
