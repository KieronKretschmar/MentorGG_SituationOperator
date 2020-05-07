using MatchEntities;
using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationOperator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationOperator
{
    /// <summary>
    /// Composition of required objects for the management of Situations that follow a specific pattern and are stored in a particular table in the database.
    /// </summary>
    /// <typeparam name="TSituation"></typeparam>
    public interface ISituationManager<TSituation> where TSituation : class, ISituation 
    {
        /// <summary>
        /// Identifies the type of situation.
        /// </summary>
        public SituationType SituationType { get; set; }

        /// <summary>
        /// Selects the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        Func<SituationContext, DbSet<TSituation>> TableSelector { get; set; }

        /// <summary>
        /// The detector for extracting occurences of TSituation.
        /// </summary>
        IPatternDetector<TSituation> Detector { get; set; }
    }
}
