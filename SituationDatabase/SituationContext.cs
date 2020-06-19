using Microsoft.EntityFrameworkCore;
using SituationDatabase.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase
{
    public class SituationContext : DbContext
    {
        public SituationContext()
        {

        }

        public SituationContext(DbContextOptions<SituationContext> options) : base(options)
        {

        }
        #region MetaData
        public virtual DbSet<MatchEntity> Match { get; set; }
        public virtual DbSet<RoundEntity> Round { get; set; }
        public virtual DbSet<PlayerMatchEntity> PlayerMatch { get; set; }
        public virtual DbSet<PlayerRoundEntity> PlayerRound { get; set; }
        #endregion

        #region Highlights - SinglePlayer
        public virtual DbSet<EffectiveHeGrenade> EffectiveHeGrenade { get; set; }
        public virtual DbSet<KillWithOwnFlashAssist> KillWithOwnFlashAssist { get; set; }
        public virtual DbSet<Clutch> Clutch { get; set; }
        public virtual DbSet<HighImpactRound> HighImpactRound { get; set; }
        public virtual DbSet<MultiKill> MultiKill { get; set; }
        public virtual DbSet<TradeKill> TradeKill { get; set; }
        public virtual DbSet<KillThroughSmoke> KillThroughSmoke { get; set; }
        public virtual DbSet<WallBangKill> WallBangKill { get; set; }
        public virtual DbSet<CollateralKill> CollateralKill { get; set; }
        public virtual DbSet<FlashAssist> FlashAssist { get; set; }
        #endregion

        #region Misplays - Singleplayer
        public virtual DbSet<SmokeFail> SmokeFail { get; set; }
        public virtual DbSet<DeathInducedBombDrop> DeathInducedBombDrop { get; set; }
        public virtual DbSet<SelfFlash> SelfFlash { get; set; }
        public virtual DbSet<TeamFlash> TeamFlash { get; set; }
        public virtual DbSet<RifleFiredWhileMoving> RifleFiredWhileMoving { get; set; }
        public virtual DbSet<UnnecessaryReload> UnnecessaryReload { get; set; }
        public virtual DbSet<PushBeforeSmokeDetonated> PushBeforeSmokeDetonated { get; set; }
        public virtual DbSet<BombDropAtSpawn> BombDropAtSpawn { get; set; }
        public virtual DbSet<HasNotBoughtDefuseKit> HasNotBoughtDefuseKit { get; set; }
        public virtual DbSet<MissedTradeKill> MissedTradeKill { get; set; }
        #endregion

        #region Keyless
        public virtual DbSet<SituationInfoByRank> RankDistribution { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region MetaData
            modelBuilder.Entity<MatchEntity>(entity =>
            {
                entity.HasKey(x => x.MatchId);

                entity.HasIndex(x => x.MatchId);

                #region TeamStats

                entity.Ignore(x => x.TeamStats);

                // TerroristStarter
                entity.Property("Score1");
                entity.Property("RealScore1");
                entity.Property("NumRoundsT1");
                entity.Property("NumRoundsCt1");
                entity.Property("BombPlants1");
                entity.Property("BombExplodes1");
                entity.Property("BombDefuses1");
                entity.Property("MoneyEarned1");
                entity.Property("MoneySpent1");

                // CtStarter
                entity.Property("Score2");
                entity.Property("RealScore2");
                entity.Property("NumRoundsT2");
                entity.Property("NumRoundsCt2");
                entity.Property("BombPlants2");
                entity.Property("BombExplodes2");
                entity.Property("BombDefuses2");
                entity.Property("MoneyEarned2");
                entity.Property("MoneySpent2");

                #endregion

            });

            modelBuilder.Entity<RoundEntity>(entity =>
            {
                entity.HasKey(x => new { x.MatchId, x.Round });

                entity.HasIndex(x => x.MatchId);

                entity.HasOne(x => x.Match)
                    .WithMany(x => x.Round)
                    .HasForeignKey(x => x.MatchId)
                    .IsRequired();
            });

            modelBuilder.Entity<PlayerMatchEntity>(entity =>
            {
                entity.HasKey(x => new { x.MatchId, x.SteamId });

                entity.HasIndex(x => x.MatchId);

                entity.HasOne(x => x.Match)
                    .WithMany(x => x.PlayerMatch)
                    .HasForeignKey(x => x.MatchId)
                    .IsRequired();
            });

            modelBuilder.Entity<PlayerRoundEntity>(entity =>
            {
                entity.HasKey(x => new { x.MatchId, x.Round, x.SteamId });

                entity.HasIndex(x => x.MatchId);

                entity.HasOne(x => x.Match)
                    .WithMany(x => x.PlayerRound)
                    .HasForeignKey(x => x.MatchId)
                    .IsRequired();

                entity.HasOne(x => x.RoundEntity)
                    .WithMany(x => x.PlayerRound)
                    .HasForeignKey(x => new { x.MatchId, x.Round })
                    .IsRequired();

                entity.HasOne(x => x.PlayerMatch)
                    .WithMany(x => x.PlayerRound)
                    .HasForeignKey(x => new { x.MatchId, x.SteamId })
                    .IsRequired();

            });
            #endregion

            #region Highlights - SinglePlayer
            modelBuilder.AddSinglePlayerSituation<EffectiveHeGrenade>("EffectiveHeGrenade");
            modelBuilder.AddSinglePlayerSituation<KillWithOwnFlashAssist>("KillWithOwnFlashAssist");
            modelBuilder.AddSinglePlayerSituation<Clutch>("Clutch");
            modelBuilder.AddSinglePlayerSituation<HighImpactRound>("HighImpactRound");
            modelBuilder.AddSinglePlayerSituation<MultiKill>("MultiKill");
            modelBuilder.AddSinglePlayerSituation<TradeKill>("TradeKill");
            modelBuilder.AddSinglePlayerSituation<KillThroughSmoke>("KillThroughSmoke");
            modelBuilder.AddSinglePlayerSituation<WallBangKill>("WallBangKill");
            modelBuilder.AddSinglePlayerSituation<CollateralKill>("CollateralKill");
            modelBuilder.AddSinglePlayerSituation<FlashAssist>("FlashAssist");
            #endregion

            #region Misplays - SinglePlayer
            modelBuilder.AddSinglePlayerSituation<SmokeFail>("SmokeFail");
            modelBuilder.AddSinglePlayerSituation<DeathInducedBombDrop>("DeathInducedBombDrop");
            modelBuilder.AddSinglePlayerSituation<SelfFlash>("SelfFlash");
            modelBuilder.AddSinglePlayerSituation<TeamFlash>("TeamFlash");
            modelBuilder.AddSinglePlayerSituation<RifleFiredWhileMoving>("RifleFiredWhileMoving");
            modelBuilder.AddSinglePlayerSituation<UnnecessaryReload>("UnnecessaryReload");
            modelBuilder.AddSinglePlayerSituation<PushBeforeSmokeDetonated>("PushBeforeSmokeDetonated");
            modelBuilder.AddSinglePlayerSituation<BombDropAtSpawn>("BombDropAtSpawn");
            modelBuilder.AddSinglePlayerSituation<HasNotBoughtDefuseKit>("HasNotBoughtDefuseKit");
            modelBuilder.AddSinglePlayerSituation<MissedTradeKill>("MissedTradeKill");
            #endregion

            #region Keyless
            modelBuilder.Entity<SituationInfoByRank>().HasNoKey();
            #endregion
        }
    }

    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Configures the modelBuilder for the given Situation, including PrimaryKeys, Indexes and ForeignKeys.
        /// </summary>
        /// <typeparam name="TSituation">Type of the Situation</typeparam>
        /// <param name="modelBuilder">ModelBuilder used in OnModelCreating</param>
        /// <param name="matchPropertySelector">Selects the navigational property from MatchEntity to this Situations table</param>
        public static void AddSituation<TSituation>(
            this ModelBuilder modelBuilder, 
            string tableName
            )
            where TSituation : class, ISituation
        {
            modelBuilder.Entity<TSituation>(entity =>
            {
                entity.HasKey(e => new { e.MatchId, e.Id });

                entity.HasIndex(e => e.MatchId);

                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasOne(x => x.Match)
                    .WithMany(tableName)
                    .HasForeignKey(x => x.MatchId)
                    .IsRequired();

                entity.HasOne(x => x.RoundEntity)
                    .WithMany(tableName)
                    .HasForeignKey(x => new { x.MatchId, x.Round })
                    .IsRequired();
            });
        }

        public static void AddSinglePlayerSituation<TSituation>(
            this ModelBuilder modelBuilder,
            string tableName
            )
            where TSituation : class, ISinglePlayerSituation
        {
            AddSituation<TSituation>(modelBuilder, tableName);

            modelBuilder.Entity<TSituation>(entity =>
            {
                entity.HasIndex(e => e.SteamId);

                entity.HasOne(x => x.PlayerMatch)
                    .WithMany(tableName)
                    .HasForeignKey(x => new { x.MatchId, x.SteamId })
                    .IsRequired();

                entity.HasOne(x => x.PlayerRound)
                    .WithMany(tableName)
                    .HasForeignKey(x => new { x.MatchId, x.Round, x.SteamId })
                    .IsRequired();
            });
        }
    }
}
