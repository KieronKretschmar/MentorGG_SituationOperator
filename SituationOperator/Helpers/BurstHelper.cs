using EquipmentLib;
using MatchEntities;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    /// <summary>
    /// Helper class for working with bursts (sprays), i.e. shots fired in rapid succession by the same weapon and player.
    /// </summary>
    public interface IBurstHelper
    {
        /// <summary>
        /// Divides WeaponFireds of a single player.
        /// </summary>
        /// <param name="weaponFireds"></param>
        /// <param name="tolerance"></param>
        /// <returns>
        /// List of bursts with bullets ordered by time. Bursts of the same player are also in ordered by time.
        /// </returns>
        List<BurstHelper.Burst> DivideIntoBursts(IEnumerable<WeaponFired> weaponFireds, MatchStats matchStats, int tolerance);
    }

    /// <inheritdoc/>
    public class BurstHelper : IBurstHelper
    {
        private readonly IEquipmentHelper _equipmentHelper;

        public BurstHelper(IEquipmentHelper equipmentHelper)
        {
            _equipmentHelper = equipmentHelper;
        }

        /// <inheritdoc/>
        public List<Burst> DivideIntoBursts(IEnumerable<WeaponFired> weaponFireds, MatchStats matchStats, int tolerance)
        {
            var equipmentDict = _equipmentHelper.GetEquipmentDict(matchStats);
            var bursts = new List<Burst>();
            var weaponFiredsByPlayerAndRound = weaponFireds
                .GroupBy(x => new { x.PlayerId, x.Round });

            foreach (var weaponFiredsByPlayerInRound in weaponFiredsByPlayerAndRound)
            {
                var forceStartNewBurst = true;
                foreach (var wf in weaponFiredsByPlayerInRound.OrderBy(x => x.Time))
                {
                    // If there are no bursts for this player and round or assigning this wf to the previous burst did not work, start a new burst.
                    var addedToPreviousBurst = forceStartNewBurst 
                        ? false 
                        : bursts.Last().TryAdd(wf);
                    
                    if (addedToPreviousBurst == false)
                    {
                        var equipmentInfo = equipmentDict[wf.Weapon];
                        bursts.Add(new Burst(wf, equipmentInfo, tolerance));
                        forceStartNewBurst = false;
                        continue;
                    }
                }
            }

            return bursts;
        }

        /// <summary>
        /// Holds a number of WeaponFireds that were fired within rapid succession by the same player.
        /// </summary>
        public class Burst
        {
            public long PlayerId { get; set; }
            public short Round { get; set; }
            public EquipmentElement Weapon { get; set; }
            public EquipmentInfo EquipmentInfo { get; set; }
            public List<WeaponFired> WeaponFireds { get; set; } = new List<WeaponFired>();

            /// <summary>
            /// Maximum velocity a player can move at to shoot accurately with this weapon. 
            /// </summary>
            private double MaxAccurateVelocity => (double)EquipmentInfo.MaxPlayerSpeed / 3;

            /// <summary>
            /// Maximum time the weapon needs between two shots.
            /// </summary>
            private double MaxAllowedTimeBetweenShots { get; set; }

            public int InaccurateBullets => WeaponFireds.Count(x => x.PlayerVelo.Length() > MaxAccurateVelocity);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="weaponFired"></param>
            /// <param name="equipmentInfo"></param>
            /// <param name="tolerance">
            /// Maximum time between the theoretical moment a weapon could have fired the next bullet and the actual moment the bullet was fired to count as a single spray.
            /// Can be used to add some extra allowed time between shots to count as burst, to allow for inaccurate time data or players tapping very quickly yet not shooting at the exact CycleTime.
            /// </param>
            public Burst(WeaponFired weaponFired, EquipmentInfo equipmentInfo, int tolerance)
            {
                PlayerId = weaponFired.PlayerId;
                Round = weaponFired.Round;
                Weapon = weaponFired.Weapon;
                WeaponFireds.Add(weaponFired);

                EquipmentInfo = equipmentInfo;
                MaxAllowedTimeBetweenShots = equipmentInfo.CycleTime + tolerance;
            }

            /// <summary>
            /// Tries to add a new weaponFired to this burst. Before calling, make sure that the new weaponfired is from the same player.
            /// </summary>
            /// <param name="wf"></param>
            /// <returns>Whether the WeaponFired was added to this burst, or wasn't because it does not belong to it.</returns>
            public bool TryAdd(WeaponFired wf)
            {
                var belongsToThisBurst =
                    wf.Weapon == Weapon
                    && WeaponFireds.Last().Time + MaxAllowedTimeBetweenShots >= wf.Time;

                if (belongsToThisBurst)
                {

                    WeaponFireds.Add(wf);
                    return true;
                }

                return false;
            }
        }
    }
}
