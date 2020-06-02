using SituationDatabase;
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
        public SituationCollection(SituationType situationType, IEnumerable<ISituation> situations)
        {
            Situations = situations.ToList();
            MetaData = new SituationTypeMetaData(situationType);
        }

        public class SituationTypeMetaData
        {
            public SituationTypeMetaData(SituationType situationType)
            {
                SituationType = situationType;
                SituationName = situationType.ToString();
            }

            /// <summary>
            /// Identifies the type of situation.
            /// </summary>
            public SituationType SituationType { get; set; }

            /// <summary>
            /// Name of the type of situation.
            /// </summary>
            public string SituationName { get; set; }
        }
    }
}

