using Microsoft.EntityFrameworkCore;
using SituationDatabase.Models;
using SituationDatabase.Models.Highlights;
using SituationDatabase.Models.Misplays;
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

        #region Misplays - Singleplayer
        public virtual DbSet<SmokeFail> SmokeFail { get; set; }
        #endregion

        #region Goodplays - SinglePlayer
        public virtual DbSet<EffectiveHeGrenade> EffectiveHeGrenade { get; set; }
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
                entity.HasKey(x => new { x.MatchId, x.RoundNumber, x.SteamId });

                entity.HasIndex(x => x.MatchId);

                entity.HasOne(x => x.Match)
                    .WithMany(x => x.PlayerRound)
                    .HasForeignKey(x => x.MatchId)
                    .IsRequired();

                entity.HasOne(x => x.Round)
                    .WithMany(x => x.PlayerRound)
                    .HasForeignKey(x => new { x.MatchId, x.RoundNumber })
                    .IsRequired();

                entity.HasOne(x => x.PlayerMatch)
                    .WithMany(x => x.PlayerRound)
                    .HasForeignKey(x => new { x.MatchId, x.SteamId })
                    .IsRequired();

            });
            #endregion

            #region Misplays - SinglePlayer
            modelBuilder.AddSinglePlayerSituation<SmokeFail>();
            #endregion

            #region Goodplays - SinglePlayer
            modelBuilder.AddSinglePlayerSituation<EffectiveHeGrenade>();
            #endregion
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void AddSituation<TSituation>(this ModelBuilder modelBuilder)
            where TSituation : class, ISituation
        {
            modelBuilder.Entity<TSituation>(entity =>
            {
                entity.HasKey(e => new { e.MatchId, e.Id });

                entity.HasIndex(e => e.MatchId);

                entity.Property(x => x.Id).ValueGeneratedOnAdd();
            });
        }

        public static void AddSinglePlayerSituation<TSituation>(this ModelBuilder modelBuilder)
            where TSituation : class, ISinglePlayerSituation
        {
            modelBuilder.Entity<TSituation>(entity =>
            {
                entity.HasKey(e => new { e.MatchId, e.Id });

                entity.Property(x => x.Id).ValueGeneratedOnAdd();

                entity.HasIndex(e => e.MatchId);

                entity.HasIndex(e => e.SteamId);
            });
        }
    }
}
