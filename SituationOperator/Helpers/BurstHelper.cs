using EquipmentLib;
using MatchEntities;
using MatchEntities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    public static class BurstHelper
    {
        /// <summary>
        /// Divides WeaponFireds of a single player
        /// </summary>
        /// <param name="weaponFireds"></param>
        /// <param name="equipmentDict"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static List<Burst> DivideIntoBursts(IEnumerable<WeaponFired> weaponFireds, Dictionary<EquipmentElement, EquipmentInfo> equipmentDict, int tolerance)
        {
            var bursts = new List<Burst>();
            var weaponFiredsByPlayerAndRound = weaponFireds
                .GroupBy(x => new { x.PlayerId, x.Round });

            foreach (var weaponFiredsByPlayerInRound in weaponFiredsByPlayerAndRound)
            {
                foreach (var wf in weaponFiredsByPlayerInRound.OrderBy(x => x.Time))
                {
                    // If there are no bursts or assigning this wf to the previous burst does not work, start a new burst. 
                    if (bursts.Count() == 0 || !bursts.Last().TryAdd(wf))
                    {
                        var equipmentInfo = equipmentDict[wf.Weapon];
                        bursts.Add(new Burst(wf, equipmentInfo, tolerance));
                        continue;
                    }
                }
            }

            return bursts;
        }

        /// <summary>
        /// 
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

            public bool TryAdd(WeaponFired wf)
            {
                if (wf.Time < WeaponFireds.Last().Time)
                {
                    throw new ArgumentException("WeaponFireds have to be added in chronological order.");
                }

                var belongsToThisBurst =
                    wf.Round == Round
                    && wf.Weapon == Weapon
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
