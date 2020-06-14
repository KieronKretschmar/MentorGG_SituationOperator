using MatchEntities;
using MatchEntities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Enums;
using SituationOperator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// A SituationManager. 
    /// See <see cref="ExtractSituationsAsync(MatchDataSet)"/> for more info regarding Situation specific logic.
    /// </summary>
    public class HasNotBoughtDefuseKitManager : SinglePlayerSituationManager<HasNotBoughtDefuseKit>
    {
        /// <summary>
        /// Minimum amount of money the player still had available after finishing his buys to count as a misplay.
        /// </summary>
        private const int MIN_MONEY_LEFT_OVER = 1000;

        /// <summary>
        /// Minimum value of the equipment the player's team carried within 10 seconds of FreezeTimeEnd to count as a misplay.
        /// 
        /// Reason: To avoid counting Ecos.
        /// </summary>
        private const int MIN_TEAM_EQUIPMENT_VALUE = 15000;

        /// <summary>
        /// Maximum number of teammates that had a defusekit to count as a misplay.
        /// 
        /// Reason: Saving money when 4 teammates have a kit is legitimate in many situations.
        /// </summary>
        private const int MAX_TEAM_DEFUSEKITS = 4;

        private readonly IServiceProvider _sp;
        private readonly ILogger<HasNotBoughtDefuseKitManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HasNotBoughtDefuseKitManager(IServiceProvider sp, ILogger<HasNotBoughtDefuseKitManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Grenades;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.HasNotBoughtDefuseKit;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<HasNotBoughtDefuseKit>> TableSelector => context => context.HasNotBoughtDefuseKit;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<HasNotBoughtDefuseKit>> ExtractSituationsAsync(MatchDataSet data)
        {
            var misplays = new List<HasNotBoughtDefuseKit>();
            using (var scope = _sp.CreateScope())
            {
                var equipmentHelper = _sp.GetRequiredService<IEquipmentHelper>();
                var equipmentDict = equipmentHelper.GetEquipmentDict(data.MatchStats);

                foreach (var round in data.RoundStatsList)
                {
                    var ctPlayerRounds = data.PlayerRoundStatsList.Where(x => x.Round == round.Round && x.IsCt);

                    // Apply MIN_TEAM_EQUIPMENT_VALUE condition
                    var lastMomentToMeasureEquipmentValue = round.StartTime + data.GetMatchSettings().FreezeTime + 10000;
                    var ctEquipmentValue = data.MaximumTeamEquipmentValue(equipmentDict, round.Round, true, endTime: lastMomentToMeasureEquipmentValue);

                    if (ctEquipmentValue <= MIN_TEAM_EQUIPMENT_VALUE)
                        continue;

                    var defusersBought = data.ItemPickedUpList
                        .Where(x => x.Round == round.Round && x.Equipment == EquipmentElement.DefuseKit && x.Buy);

                    var defusersSavedFromLastRound = data.ItemSavedList
                        .Where(x => x.Equipment == EquipmentElement.DefuseKit && x.Round == round.Round);

                    // Apply MAX_TEAM_DEFUSEKITS condition
                    if (defusersBought.Count() + defusersSavedFromLastRound.Count() >= MAX_TEAM_DEFUSEKITS)
                        continue;

                    foreach (var ctPlayerRound in ctPlayerRounds)
                    {
                        var steamId = ctPlayerRound.PlayerId;

                        // If the player had a kit, continue
                        if (defusersBought.Any(x => x.PlayerId == steamId) || defusersSavedFromLastRound.Any(x => x.PlayerId == steamId))
                            continue;

                        // Apply MIN_MONEY_LEFT_OVER condition
                        var moneyLeftAfterSpending = ctPlayerRound.MoneyInitial - ctPlayerRound.MoneySpent;
                        if (moneyLeftAfterSpending < MIN_MONEY_LEFT_OVER)
                            continue;

                        misplays.Add(new HasNotBoughtDefuseKit(round, steamId, defusersBought.Count() + defusersSavedFromLastRound.Count(), moneyLeftAfterSpending, ctEquipmentValue, ctPlayerRound.PlayedEquipmentValue));
                    }
                }
            }

            return misplays;
        }
    }
}
