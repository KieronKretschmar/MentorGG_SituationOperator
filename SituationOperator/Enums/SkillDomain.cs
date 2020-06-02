using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Enums
{
    /// <summary>
    /// Each SituationType belongs to a SkillDomain, i.e. an area of expertise.
    /// </summary>
    public enum SkillDomain : byte
    {
        /// <summary>
        /// The SkillDomain of not doing stupid shit.
        /// </summary>
        Tactical = 1,

        /// <summary>
        /// Skills related to grenade usage.
        /// </summary>
        Grenades = 2,

        /// <summary>
        /// Aiming, Fighting and killing.
        /// </summary>
        Shooting = 3,

        /// <summary>
        /// The art of pressing W,A,S,D.
        /// </summary>
        Movement = 4,
    }
}
