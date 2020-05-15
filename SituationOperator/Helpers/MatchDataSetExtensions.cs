using MatchEntities;
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
        public static RoundStats RoundStats(this MatchDataSet matchData, IRoundEntity entity)
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
        /// Determines whether the player was alive at the specified moment, including the exact moment.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool IsAlive(this MatchDataSet data, long steamId, int time)
        {
            var round = data.GetRoundByTime(time);

            var kill = data.KillList.SingleOrDefault(x => x.VictimId == steamId && x.Round == round.Round);

            if (kill == null || kill.Time > time)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines the Kill in which the player died in the specified round, or null if he survived.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamId"></param>
        /// <returns></returns>
        public static Kill Death(this MatchDataSet data, long steamId, int round)
        {
            var death = data.KillList.SingleOrDefault(x => x.VictimId == steamId && x.Round == round);
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
            return data.PlayerPositionList
                .Where(x => x.PlayerId == steamId)
                .OrderByDescending(x => x.Time)
                .First(x => x.Time < time);
        }

        /// <summary>
        /// Returns the distance of the last known positions of both specified players before the given time.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="steamIdFirst"></param>
        /// <param name="steamIdSecond"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float LastPositionDistance(this MatchDataSet data, long steamIdFirst, long steamIdSecond, int time)
        {
            var firstPos = data.LastPlayerPos(steamIdFirst, time).PlayerPos;
            var secondPos = data.LastPlayerPos(steamIdSecond, time).PlayerPos;

            var dist = Vector3.Distance(firstPos, secondPos);

            return dist;
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
