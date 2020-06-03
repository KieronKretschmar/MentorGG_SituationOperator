﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SituationDatabase;

namespace SituationDatabase.Migrations
{
    [DbContext(typeof(SituationContext))]
    [Migration("20200511140038_initial-migration")]
    partial class initialmigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SituationDatabase.Models.EffectiveHeGrenade", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("EnemiesHit")
                        .HasColumnType("int");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<int>("TotalEnemyDamage")
                        .HasColumnType("int");

                    b.Property<int>("TotalTeamDamage")
                        .HasColumnType("int");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.ToTable("EffectiveHeGrenade");
                });

            modelBuilder.Entity("SituationDatabase.Models.SmokeFail", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("LineupId")
                        .HasColumnType("int");

                    b.Property<string>("LineupName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.ToTable("SmokeFail");
                });
#pragma warning restore 612, 618
        }
    }
}