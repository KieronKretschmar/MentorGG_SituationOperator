using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Enums
{
    /// <summary>
    /// Identifies collections of SituationTypes.
    /// </summary>
    public enum SituationTypeCollection : byte
    {
        /// <summary>
        /// Identifies the collection of all SituationTypes that are analyzed in production environment.
        /// </summary>
        ProductionExtractionDefault = 1,

        /// <summary>
        /// Identifies the collection of all SituationTypes that are accessible by all users.
        /// </summary>
        ProductionAccessDefault = 2
    }
}
