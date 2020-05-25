﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SituationDatabase;

namespace SituationDatabase.Migrations
{
    [DbContext(typeof(SituationContext))]
    partial class SituationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SituationDatabase.Models.DeathInducedBombDrop", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<float>("ClosestTeammateDistance")
                        .HasColumnType("float");

                    b.Property<int>("PickedUpAfter")
                        .HasColumnType("int");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<int>("TeammatesAlive")
                        .HasColumnType("int");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("DeathInducedBombDrop");
                });

            modelBuilder.Entity("SituationDatabase.Models.EffectiveHeGrenade", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("EnemiesHit")
                        .HasColumnType("int");

                    b.Property<int>("EnemiesKilled")
                        .HasColumnType("int");

                    b.Property<long>("GrenadeId")
                        .HasColumnType("bigint");

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

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("EffectiveHeGrenade");
                });

            modelBuilder.Entity("SituationDatabase.Models.KillWithOwnFlashAssist", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("AssistedKills")
                        .HasColumnType("int");

                    b.Property<long>("GrenadeId")
                        .HasColumnType("bigint");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<int>("TimeBetweenDetonationAndFirstKill")
                        .HasColumnType("int");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("KillWithOwnFlashAssist");
                });

            modelBuilder.Entity("SituationDatabase.Models.MatchEntity", b =>
                {
                    b.Property<long>("MatchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<float?>("AvgRank")
                        .HasColumnType("float");

                    b.Property<short>("BombDefuses1")
                        .HasColumnType("smallint");

                    b.Property<short>("BombDefuses2")
                        .HasColumnType("smallint");

                    b.Property<short>("BombExplodes1")
                        .HasColumnType("smallint");

                    b.Property<short>("BombExplodes2")
                        .HasColumnType("smallint");

                    b.Property<short>("BombPlants1")
                        .HasColumnType("smallint");

                    b.Property<short>("BombPlants2")
                        .HasColumnType("smallint");

                    b.Property<byte>("GameType")
                        .HasColumnType("tinyint unsigned");

                    b.Property<string>("Map")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("MatchDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("MoneyEarned1")
                        .HasColumnType("int");

                    b.Property<int>("MoneyEarned2")
                        .HasColumnType("int");

                    b.Property<int>("MoneySpent1")
                        .HasColumnType("int");

                    b.Property<int>("MoneySpent2")
                        .HasColumnType("int");

                    b.Property<short>("NumRoundsCt1")
                        .HasColumnType("smallint");

                    b.Property<short>("NumRoundsCt2")
                        .HasColumnType("smallint");

                    b.Property<short>("NumRoundsT1")
                        .HasColumnType("smallint");

                    b.Property<short>("NumRoundsT2")
                        .HasColumnType("smallint");

                    b.Property<short>("RealScore1")
                        .HasColumnType("smallint");

                    b.Property<short>("RealScore2")
                        .HasColumnType("smallint");

                    b.Property<short>("Score1")
                        .HasColumnType("smallint");

                    b.Property<short>("Score2")
                        .HasColumnType("smallint");

                    b.Property<byte>("Source")
                        .HasColumnType("tinyint unsigned");

                    b.Property<byte>("WinnerTeam")
                        .HasColumnType("tinyint unsigned");

                    b.HasKey("MatchId");

                    b.HasIndex("MatchId");

                    b.ToTable("Match");
                });

            modelBuilder.Entity("SituationDatabase.Models.PlayerMatchEntity", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<short>("AssistCount")
                        .HasColumnType("smallint");

                    b.Property<short>("DeathCount")
                        .HasColumnType("smallint");

                    b.Property<short>("KillCount")
                        .HasColumnType("smallint");

                    b.Property<short>("Mvps")
                        .HasColumnType("smallint");

                    b.Property<short>("Score")
                        .HasColumnType("smallint");

                    b.Property<byte>("Team")
                        .HasColumnType("tinyint unsigned");

                    b.HasKey("MatchId", "SteamId");

                    b.HasIndex("MatchId");

                    b.ToTable("PlayerMatch");
                });

            modelBuilder.Entity("SituationDatabase.Models.PlayerRoundEntity", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<byte>("ArmorType")
                        .HasColumnType("tinyint unsigned");

                    b.Property<bool>("IsCt")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("MoneyInitial")
                        .HasColumnType("int");

                    b.Property<int>("PlayedEquipmentValue")
                        .HasColumnType("int");

                    b.HasKey("MatchId", "Round", "SteamId");

                    b.HasIndex("MatchId");

                    b.HasIndex("MatchId", "SteamId");

                    b.ToTable("PlayerRound");
                });

            modelBuilder.Entity("SituationDatabase.Models.PushBeforeSmokeDetonated", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("GrenadeId")
                        .HasColumnType("bigint");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("SmokeDetonationTime")
                        .HasColumnType("int");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("PushBeforeSmokeDetonated");
                });

            modelBuilder.Entity("SituationDatabase.Models.RifleFiredWhileMoving", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("Bullets")
                        .HasColumnType("int");

                    b.Property<int>("InaccurateBullets")
                        .HasColumnType("int");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<short>("Weapon")
                        .HasColumnType("smallint");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("RifleFiredWhileMoving");
                });

            modelBuilder.Entity("SituationDatabase.Models.RoundEntity", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<bool>("BombPlanted")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("EndTime")
                        .HasColumnType("int");

                    b.Property<bool>("OriginalSide")
                        .HasColumnType("tinyint(1)");

                    b.Property<long?>("PlayerMatchMatchId")
                        .HasColumnType("bigint");

                    b.Property<long?>("PlayerMatchSteamId")
                        .HasColumnType("bigint");

                    b.Property<int>("RealEndTime")
                        .HasColumnType("int");

                    b.Property<int>("RoundTime")
                        .HasColumnType("int");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<byte>("WinType")
                        .HasColumnType("tinyint unsigned");

                    b.Property<byte>("WinnerTeam")
                        .HasColumnType("tinyint unsigned");

                    b.HasKey("MatchId", "Round");

                    b.HasIndex("MatchId");

                    b.HasIndex("PlayerMatchMatchId", "PlayerMatchSteamId");

                    b.ToTable("Round");
                });

            modelBuilder.Entity("SituationDatabase.Models.SelfFlash", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("AngleToCrosshairSelf")
                        .HasColumnType("int");

                    b.Property<int?>("DeathTimeSelf")
                        .HasColumnType("int");

                    b.Property<long>("GrenadeId")
                        .HasColumnType("bigint");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<int>("TimeFlashedEnemies")
                        .HasColumnType("int");

                    b.Property<int>("TimeFlashedSelf")
                        .HasColumnType("int");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("SelfFlash");
                });

            modelBuilder.Entity("SituationDatabase.Models.SmokeFail", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("GrenadeId")
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

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("SmokeFail");
                });

            modelBuilder.Entity("SituationDatabase.Models.TeamFlash", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("FlashedTeammates")
                        .HasColumnType("int");

                    b.Property<int>("FlashedTeammatesDeaths")
                        .HasColumnType("int");

                    b.Property<long>("GrenadeId")
                        .HasColumnType("bigint");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<int>("TimeFlashedEnemies")
                        .HasColumnType("int");

                    b.Property<int>("TimeFlashedTeammates")
                        .HasColumnType("int");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("TeamFlash");
                });

            modelBuilder.Entity("SituationDatabase.Models.UnnecessaryReload", b =>
                {
                    b.Property<long>("MatchId")
                        .HasColumnType("bigint");

                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("AmmoBefore")
                        .HasColumnType("int");

                    b.Property<short>("Round")
                        .HasColumnType("smallint");

                    b.Property<int>("StartTime")
                        .HasColumnType("int");

                    b.Property<long>("SteamId")
                        .HasColumnType("bigint");

                    b.Property<short>("Weapon")
                        .HasColumnType("smallint");

                    b.HasKey("MatchId", "Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("SteamId");

                    b.HasIndex("MatchId", "SteamId");

                    b.HasIndex("MatchId", "Round", "SteamId");

                    b.ToTable("UnnecessaryReload");
                });

            modelBuilder.Entity("SituationDatabase.Models.DeathInducedBombDrop", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("DeathInducedBombDrop")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("DeathInducedBombDrop")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("DeathInducedBombDrop")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("DeathInducedBombDrop")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.EffectiveHeGrenade", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("EffectiveHeGrenade")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("EffectiveHeGrenade")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("EffectiveHeGrenade")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("EffectiveHeGrenade")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.KillWithOwnFlashAssist", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("KillWithOwnFlashAssist")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("KillWithOwnFlashAssist")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("KillWithOwnFlashAssist")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("KillWithOwnFlashAssist")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.PlayerMatchEntity", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("PlayerMatch")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.PlayerRoundEntity", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("PlayerRound")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("PlayerRound")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("PlayerRound")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.PushBeforeSmokeDetonated", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("PushBeforeSmokeDetonated")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("PushBeforeSmokeDetonated")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("PushBeforeSmokeDetonated")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("PushBeforeSmokeDetonated")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.RifleFiredWhileMoving", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("RifleFiredWhileMoving")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("RifleFiredWhileMoving")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("RifleFiredWhileMoving")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("RifleFiredWhileMoving")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.RoundEntity", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("Round")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("Round")
                        .HasForeignKey("PlayerMatchMatchId", "PlayerMatchSteamId");
                });

            modelBuilder.Entity("SituationDatabase.Models.SelfFlash", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("SelfFlash")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("SelfFlash")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("SelfFlash")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("SelfFlash")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.SmokeFail", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("SmokeFail")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("SmokeFail")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("SmokeFail")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("SmokeFail")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.TeamFlash", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("TeamFlash")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("TeamFlash")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("TeamFlash")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("TeamFlash")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SituationDatabase.Models.UnnecessaryReload", b =>
                {
                    b.HasOne("SituationDatabase.Models.MatchEntity", "Match")
                        .WithMany("UnnecessaryReload")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.RoundEntity", "RoundEntity")
                        .WithMany("UnnecessaryReload")
                        .HasForeignKey("MatchId", "Round")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerMatchEntity", "PlayerMatch")
                        .WithMany("UnnecessaryReload")
                        .HasForeignKey("MatchId", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SituationDatabase.Models.PlayerRoundEntity", "PlayerRound")
                        .WithMany("UnnecessaryReload")
                        .HasForeignKey("MatchId", "Round", "SteamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
