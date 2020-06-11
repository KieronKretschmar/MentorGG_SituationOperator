using MatchEntities;
using MatchEntities.Enums;
using MatchEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    public static class MatchDataSetExtensions
    {
        #region General / Meta data
        /// <summary>
        /// Gets the RoundStats object referenced by the given entity.
        /// </summary>
        /// <param name="matchData"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static RoundStats GetRoundStats(this MatchDataSet matchData, IRoundEntity entity)
        {
            return matchData.RoundStatsList.Single(x => x.Round == entity.Round);
        }

        public static RoundStats GetRoundByTime(this MatchDataSet data, int time)
        {
            return data.RoundStatsList
                .OrderByDescending(x => x.StartTime)
                .First(x => x.StartTime < time);
        }

        /// <summary>
        /// Returns the PlayerRoundStats entries of the winners/losers of the specified round.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="round"></param>
        /// <param name="selectWinners">Whether to select the winners or losers</param>
        /// <returns></returns>
        public static List<PlayerRoundStats> GetPlayerRoundStatsByRoundOutcome(this MatchDataSet data, short round, bool selectWinners)
        {
            var winnerTeam = data.RoundStatsList.Single(x => x.Round == round).WinnerTeam;
            var ctsWon = winnerTeam == StartingFaction.CtStarter;
            var selectCt = selectWinners == ctsWon;

            return data.PlayerRoundStatsList
                .Where(x => x.Round == round)
                .Where(x => selectCt == x.IsCt)
                .ToList();
        }

        /// <summary>
        /// Helper method to return a player's teammates PlayerRoundStats in a given round, including his own. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static List<PlayerRoundStats> GetTeamRoundStats(this MatchDataSet data, IPlayerEvent playerEvent)
        {
            var teammatesThisRound = data.PlayerRoundStatsList
                .Where(x => x.Round == playerEvent.Round && x.IsCt == playerEvent.IsCt)
                .ToList();

            return teammatesThisRound;
        }

        /// <summary>
        /// Helper method to return a player's teammates PlayerRoundStats in a given round. 
        /// 
        /// Preferably use override with IPlayerEvent param, if available, as it's more efficient.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="round"></param>
        /// <returns></returns>
        public static List<PlayerRoundStats> GetTeammateRoundStats(this MatchDataSet data, long steamId, short round)
        {
            var playerTeam = data.PlayerMatchStatsList.Single(x => x.SteamId == steamId).Team;

            var teammateSteamIds = data.PlayerMatchStatsList
                .Where(x => x.Team == playerTeam)
                .Select(x => x.SteamId);

            var teammatesThisRound = data.PlayerRoundStatsList
                .Where(x => x.Round == round && teammateSteamIds.Contains(x.PlayerId))
                .ToList();

            return teammatesThisRound;
        }

        /// <summary>
        /// WARNING: Incomplete / Inaccurate for providers other than Valve.
        /// See https://counterstrike.fandom.com/wiki/Competitive.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MatchRules GetMatchSettings(this MatchDataSet data)
        {
            switch (data.MatchStats.Source)
            {
                case MatchEntities.Enums.Source.Valve:
                    return new MatchRules(15000, 115000, 40000);
                default:
                    // Default to MatchMaking rules
                    return new MatchRules(15000, 115000, 40000);
            }
        }

        /// <summary>
        /// Determines whether the player was alive at the specified moment, including the exact moment.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsAlive(this MatchDataSet data, long steamId, short round, int time)
        {
            if (data.PlayerRoundStatsList.Any(x=>x.Round == round && x.PlayerId == steamId) == false)
            {
                return false;
            }

            var death = data.Death(steamId, round);

            if (death == null || death.Time > time)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines the Kill in which the player died in the specified round, or null if he survived.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <returns></returns>
        public static Kill Death(this MatchDataSet data, long steamId, int round)
        {
            // Use FirstOrDefault instead of SingleOrDefault because there is a known issue where a player apparently dies more than once in a single Round
            // Assuming the first death is the real one, and the second one is not because it might have happened after taking over a bot. 
            // For more info on the issue see https://gitlab.com/mentorgg/csgo/demofileworker/-/issues/12
            var death = data.KillList.FirstOrDefault(x => x.VictimId == steamId && x.Round == round);
            return death;
        }

        #endregion

        #region Position related

        /// <summary>
        /// Returns the last known position of the given player
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static PlayerPosition LastPlayerPos(this MatchDataSet data, long steamId, int time)
        {
            var pos = data.PlayerPositionList
                .Where(x => x.PlayerId == steamId)
                .OrderByDescending(x => x.Time)
                .FirstOrDefault(x => x.Time < time);

            return pos;
        }

        /// <summary>
        /// Returns the distance of the last known positions of both specified players before the given time.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamIdFirst"></param>
        /// <param name="steamIdSecond"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float? LastPositionDistance(this MatchDataSet data, long steamIdFirst, long steamIdSecond, int time)
        {
            var firstPos = data.LastPlayerPos(steamIdFirst, time);
            var secondPos = data.LastPlayerPos(steamIdSecond, time);

            if (firstPos == null || secondPos == null)
                return null;

            var dist = Vector3.Distance(firstPos.PlayerPos, secondPos.PlayerPos);
            return dist;
        }

        #endregion

        #region Item related
        /// <summary>
        /// Returns the time the given item was obtained the last time before <paramref name="time"/>, or null if it's not held by the player at the time.
        /// 
        /// If the item was saved from last round, returns time of RoundStart.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="item"></param>
        /// <param name="round"></param>
        /// <param name="time">If null, </param>
        /// <returns></returns>
        public static int? ObtainedItemTime(this MatchDataSet data, long steamId, short round, EquipmentElement item, int? time = null)
        {
            var lastPickupTime = data.ItemPickedUpList
                .Where(x =>
                    x.Round == round
                    && x.PlayerId == steamId
                    && x.Equipment == item
                    && (time == null || x.Time < time))
                .OrderByDescending(x => x.Time)
                .FirstOrDefault()
                ?.Time;

            if(lastPickupTime == null)
            {
                var hadFromRoundStart = data.ItemSavedList
                .Any(x =>
                    x.Round == round
                    && x.PlayerId == steamId
                    && x.Equipment == EquipmentElement.Bomb);
                if (hadFromRoundStart)
                {
                    lastPickupTime = data.RoundStatsList.Single(x => x.Round == round).StartTime;
                }
            }

            var droppedAfterPickup = data.ItemDroppedList
                .Any(x =>
                    x.Round == round
                    && x.PlayerId == steamId
                    && x.Equipment == item
                    && (time == null || x.Time < time));

            return droppedAfterPickup ? null : lastPickupTime;
        }
        #endregion

        #region Weapon related
        /// <summary>
        /// Indicates whether the player dealt or took damage in the indicated timeframe.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="round"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static bool PlayerDealtOrTookDamage(this MatchDataSet data, long steamId, short? round = null, int? startTime = null, int? endTime = null)
        {
            var firstDamageDealt = data.FirstDamageDealt(steamId, round, startTime, endTime);
            var firstDamageTaken = data.FirstDamageDealt(steamId, round, startTime, endTime);

            return firstDamageDealt == null && firstDamageTaken == null;
        }

        /// <summary>
        /// Returns the first Damage the player dealt in the specified timeframe.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="round"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static Damage FirstDamageDealt(this MatchDataSet data, long steamId, short? round = null, int? startTime = null, int? endTime = null)
        {
            return data.DamageList
                .Where(x => x.PlayerId == steamId
                && startTime <= x.Time
                && (round == null || x.Round == round)
                && (startTime == null || x.Time <= startTime)
                && (endTime == null || x.Time <= endTime))
                .OrderBy(x => x.Time)
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the first Damage the player took in the specified timeframe.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="round"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static Damage FirstDamageTaken(this MatchDataSet data, long steamId, short? round = null, int? startTime = null, int? endTime = null)
        {
            return data.DamageList
                .Where(x => x.VictimId == steamId 
                && startTime <= x.Time 
                && (round == null || x.Round == round)
                && (startTime == null || x.Time <= startTime)
                && (endTime == null || x.Time <= endTime))
                .OrderBy(x => x.Time)
                .FirstOrDefault();
        }
        #endregion

        #region Grenade related
        /// <summary>
        /// Gets the Damage entities dealt by a HE grenade.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<Damage> DamagesByHe(this MatchDataSet data, He entity)
        {
            return data.DamageList.Where(x => x.HeGrenadeId == entity.GrenadeId).ToList();
        }

        /// <summary>
        /// Returns all Flashed event caused by the given flash.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<Flashed> FlashedsByFlash(this MatchDataSet data, Flash entity)
        {
            return data.FlashedList.Where(x => x.GrenadeId == entity.GrenadeId).ToList();
        }

        /// <summary>
        /// Returns the Flash that caused the given Flashed event.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Flash FlashFromFlashed(this MatchDataSet data, Flashed entity)
        {
            return data.FlashList.Where(x => x.GrenadeId == entity.GrenadeId).Single();
        }

        /// <summary>
        /// Returns all the Flashed events affecting the player at the specified round timespan.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="round">If possible, specify for increased performance when looking for particular timeframe.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static List<Flashed> GetFlasheds(this MatchDataSet data, long steamId, short? round, int? startTime = null, int? endTime = null)
        {
            var flasheds = new List<Flashed>();

            foreach (var flashed in data.FlashedList.Where(x => x.VictimId == steamId))
            {
                if (round != null && flashed.Round != round)
                    continue;

                var flash = data.FlashFromFlashed(flashed);

                if (startTime != null && flash.Time < startTime)
                    continue;

                if (endTime != null && flash.Time + flashed.TimeFlashed < endTime)
                    continue;

                flasheds.Add(flashed);
            }

            return flasheds;
        }

        #endregion
    }
}
