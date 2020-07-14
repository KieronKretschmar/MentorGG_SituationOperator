using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Models
{
    public class SituationTypeMetaData
    {
        public SituationTypeMetaData(SituationType situationType, SkillDomain skillDomain)
        {
            SituationType = situationType;
            SkillDomain = skillDomain;
        }

        /// <summary>
        /// Identifies the type of situation.
        /// </summary>
        public SituationType SituationType { get; set; }

        /// <summary>
        /// Name of the type of situation.
        /// </summary>
        public string SituationName => SituationType.ToString();

        /// <summary>
        /// The area of expertise a SituationType belongs to.
        /// </summary>
        public SkillDomain SkillDomain { get; set; }

        /// <summary>
        /// Name of the type of SkillDomain.
        /// </summary>
        public string SkillDomainName => SkillDomain.ToString();
    }
}
