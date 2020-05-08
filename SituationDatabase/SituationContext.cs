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

            modelBuilder.Entity<SmokeFail>(entity =>
            {
                entity.HasKey(e => new { e.MatchId, e.Id });

                entity.HasIndex(e => e.MatchId);

                entity.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<EffectiveHeGrenade>(entity =>
            {
                entity.HasKey(e => new { e.MatchId, e.Id });

                entity.HasIndex(e => e.MatchId);

                entity.Property(x => x.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
