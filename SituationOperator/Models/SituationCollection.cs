using SituationDatabase;
using SituationDatabase.Enums;
using SituationOperator.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SituationOperator.Models
{

    /// <summary>
    /// Holds a collection of a particular type of Situations.
    /// </summary>
    public class SituationCollection
    {

        /// <summary>
        /// Identifies the type of situation.
        /// </summary>
        public SituationTypeMetaData MetaData { get; set; }

        /// <summary>
        /// Collection of situations.
        /// </summary>
        public List<ISituation> Situations { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SituationCollection(SituationType situationType, SkillDomain skillDomain, IEnumerable<ISituation> situations)
        {
            Situations = situations.ToList();
            MetaData = new SituationTypeMetaData(situationType, skillDomain);
        }
    }
}

