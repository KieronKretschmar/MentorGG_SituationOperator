using MatchEntities;
using MatchEntities.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RabbitCommunicationLib.Interfaces;
using RabbitCommunicationLib.TransferModels;
using RabbitMQ.Client.Events;
using SituationOperator.Communications;
using SituationOperator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationOperatorTestProject
{
    [TestClass]
    public class MatchDataSetExtensionTest
    {
        /// <summary>
        /// Tests whether inventory related methods work.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task InventoryTests()
        {
            // ARRANGE
            // Initialize relevant points in time
            var round1StartTime = 1000;
            var checkpointDelta = 50;
            var round1Checkpoint1 = round1StartTime + 1 * checkpointDelta;
            var round1Checkpoint2 = round1StartTime + 2 * checkpointDelta;
            var round1Checkpoint3 = round1StartTime + 3 * checkpointDelta;

            // Initialize RoundStats
            var roundStat1 = new RoundStats
            {
                Round = 1,
                StartTime = round1StartTime
            };
            var roundStats = new List<RoundStats> { roundStat1 };

            // Initialize PlayerRoundStats
            var ct1 = new PlayerRoundStats
            {
                PlayerId = 1,
                Round = roundStat1.Round,
                IsCt = true
            };
            var ct2 = new PlayerRoundStats
            {
                PlayerId = 2,
                Round = roundStat1.Round,
                IsCt = true
            };
            var t1 = new PlayerRoundStats
            {
                PlayerId = 3,
                Round = roundStat1.Round,
                IsCt = false
            };
            var playerRoundStats = new List<PlayerRoundStats> { ct1, ct2, t1 };

            // Initialize ItemSaveds
            var awpSavedCt1 = new ItemSaved
            {
                PlayerId = ct1.PlayerId,
                Equipment = EquipmentElement.AWP,
                Round = roundStat1.Round,
                ItemId = 1,
            };
            var akSavedT1 = new ItemSaved
            {
                PlayerId = t1.PlayerId,
                Equipment = EquipmentElement.AK47,
                Round = roundStat1.Round,
                ItemId = 2,
            };
            var itemSaveds = new List<ItemSaved> { awpSavedCt1, akSavedT1 };

            // Initialize ItemDroppeds
            var awpDropCt1 = new ItemDropped
            {
                PlayerId = ct1.PlayerId,
                Equipment = EquipmentElement.AWP,
                Round = roundStat1.Round,
                ItemId = awpSavedCt1.ItemId,
                Time = round1Checkpoint1
            };
            var itemDroppeds = new List<ItemDropped> { awpDropCt1 };

            // Initialize ItemPickedUps
            var p250BuyT2 = new ItemPickedUp
            {
                PlayerId = t1.PlayerId,
                Equipment = EquipmentElement.P250,
                Round = roundStat1.Round,
                ItemId = 4,
                Time = round1Checkpoint2,
                Buy = true
            };
            var awpPickupCt2 = new ItemPickedUp
            {
                PlayerId = ct2.PlayerId,
                Equipment = awpDropCt1.Equipment,
                Round = roundStat1.Round,
                ItemId = awpDropCt1.ItemId,
                Time = round1Checkpoint2,
                Buy = false
            };
            var deagleBuyCt2 = new ItemPickedUp
            {
                PlayerId = ct2.PlayerId,
                Equipment = EquipmentElement.Deagle,
                Round = roundStat1.Round,
                ItemId = 3,
                Time = round1Checkpoint3,
                Buy = true
            };
            var itemPickedUps = new List<ItemPickedUp> { awpPickupCt2, deagleBuyCt2, p250BuyT2 };

            var matchDataSet = new MatchDataSet
            {
                RoundStatsList = roundStats,
                PlayerRoundStatsList = playerRoundStats,
                ItemSavedList = itemSaveds,
                ItemDroppedList = itemDroppeds,
                ItemPickedUpList = itemPickedUps
            };

            var eqDict = TestHelper.GetEquipmentDict(Source.Valve, DateTime.Parse("2020-06-13"));

            // ACT
            var ct1InventoryBetweenCheckPoints1And2 = matchDataSet.Inventory(roundStat1.Round, round1Checkpoint1 + checkpointDelta / 2, ct1.PlayerId);
            var t1InventoryBetweenCheckPoints1And2 = matchDataSet.Inventory(roundStat1.Round, round1Checkpoint1 + checkpointDelta / 2, t1.PlayerId);
            var ct2InventoryBetweenCheckPoints2And3 = matchDataSet.Inventory(roundStat1.Round, round1Checkpoint2 + checkpointDelta / 2, ct2.PlayerId);
            var ctMaxEquipmentValue = matchDataSet.MaximumTeamEquipmentValue(eqDict, roundStat1.Round, true);
            var ctMaxEquipmentValueBetweenCheckPoints2And3 = matchDataSet.MaximumTeamEquipmentValue(eqDict, roundStat1.Round, true, startTime: round1Checkpoint2, endTime: round1Checkpoint2 + checkpointDelta / 2);

            // ASSERT
            CollectionAssert.AreEquivalent(new List<EquipmentElement> { }, ct1InventoryBetweenCheckPoints1And2);
            CollectionAssert.AreEquivalent(new List<EquipmentElement> { EquipmentElement.AK47 }, t1InventoryBetweenCheckPoints1And2);
            CollectionAssert.AreEquivalent(new List<EquipmentElement> { EquipmentElement.AWP }, ct2InventoryBetweenCheckPoints2And3);
            Assert.AreEqual(eqDict[EquipmentElement.AWP].Price + eqDict[EquipmentElement.Deagle].Price, ctMaxEquipmentValue);
            Assert.AreEqual(eqDict[EquipmentElement.AWP].Price, ctMaxEquipmentValueBetweenCheckPoints2And3);
        }
    }
}
