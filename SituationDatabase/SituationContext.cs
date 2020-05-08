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
        public virtual DbSet<SmokeFail> SmokeFail { get; set; }
        public virtual DbSet<EffectiveHeGrenade> EffectiveHeGrenade { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
            where TSituation : class, ISinglePlayerAction
        {
            modelBuilder.Entity<TSituation>(entity =>
            {
                entity.HasKey(e => new { e.MatchId, e.Id });

                entity.HasIndex(e => e.MatchId);

                entity.HasIndex(e => e.SteamId);

                entity.Property(x => x.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
