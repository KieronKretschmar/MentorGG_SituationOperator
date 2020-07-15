using SituationDatabase;
using SituationOperator.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SituationOperator.Models
{

    /// <summary>
    /// Holds a collection of a particular type of Situations.
    /// </summary>
    public class MetaDataCollection
    {

        /// <summary>
        /// Holds SituationTypeMetaData for every SituationType.
        /// </summary>
        public List<SituationTypeMetaData> Data { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MetaDataCollection(List<SituationTypeMetaData> data)
        {
            Data = data;
        }

    }
}

