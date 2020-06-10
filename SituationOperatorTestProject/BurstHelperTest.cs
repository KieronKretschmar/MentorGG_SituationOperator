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
    public class BurstHelperTest
    {
        /// <summary>
        /// Tests whether RabbitConsumer calls the right methods and scoped dependencies are disposed.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Test()
        {
            // ARRANGE
            // Setup mocked BurstHelper
            var spHelper = new MockServiceProviderHelper();
            spHelper.AddHelperServices();
            var burstHelper = spHelper.ServiceProviderMock.Object.GetRequiredService<IBurstHelper>();
            var equipmentHelper = spHelper.ServiceProviderMock.Object.GetRequiredService<IEquipmentHelper>();

            var burstTolerance = 100;

            var weaponFireds = new List<WeaponFired>();

            // Add burst of first player
            var playerId1 = 1;
            var weapon1 = EquipmentElement.AK47;
            var weapon1Info = equipmentHelper.GetEquipmentInfo(weapon1, Source.Valve, DateTime.Now);
            var burst1StartTime = 0;
            short burst1Round = 2;
            var burst1Bullets = 15;
            var burst1WeaponFireds = CreateBurstWeaponFireds(playerId1, burst1Round, burst1StartTime, burst1Bullets, weapon1, (int)weapon1Info.CycleTime, (int)weapon1Info.CycleTime + burstTolerance);
            weaponFireds.AddRange(burst1WeaponFireds);

            // Add burst of second player
            var playerId2 = 2;
            var weapon2 = EquipmentElement.M4A1;
            var weapon2Info = equipmentHelper.GetEquipmentInfo(weapon2, Source.Valve, DateTime.Now);
            var burst2StartTime = 20000;
            short burst2Round = 1;
            var burst2Bullets = 5;
            var burst2WeaponFireds = CreateBurstWeaponFireds(playerId2, burst2Round, burst2StartTime, burst2Bullets, weapon2, (int)weapon2Info.CycleTime, (int)weapon2Info.CycleTime + burstTolerance);
            weaponFireds.AddRange(burst2WeaponFireds);

            // Add non-burst shots ... 
            // ... first burst if it weren't for another weapon
            weaponFireds.Add(CreateWeaponFired(playerId1, 1, weapon2, burst1WeaponFireds.Last().Time + 1));
            // ... first burst if it weren't for the time
            weaponFireds.Add(CreateWeaponFired(playerId1, 1, weapon2, burst1WeaponFireds.Last().Time + 100000));
            // ... first burst if it weren't for the playerId
            weaponFireds.Add(CreateWeaponFired(playerId1 + 10, 1, weapon1, burst1WeaponFireds.Last().Time));

            // ACT
            var bursts = burstHelper.DivideIntoBursts(weaponFireds, Source.Valve, DateTime.Now, burstTolerance);

            // ASSERT
            var firstBurstIncluded = bursts.Any(x => x.PlayerId == playerId1 && x.WeaponFireds.Count == burst1WeaponFireds.Count() && x.WeaponFireds.First().Time == burst1WeaponFireds.First().Time);
            Assert.IsTrue(firstBurstIncluded);
            var secondBurstIncluded = bursts.Any(x => x.PlayerId == playerId2 && x.WeaponFireds.Count == burst2WeaponFireds.Count() && x.WeaponFireds.First().Time == burst2WeaponFireds.First().Time);
            Assert.IsTrue(firstBurstIncluded);
        }

        private List<WeaponFired> CreateBurstWeaponFireds(long playerId, short round, int startTime, int bullets, EquipmentElement weapon, int minTimeBetweenShots, int maxTimeBetweenShots)
        {
            var rnd = new Random();
            var weaponFireds = new List<WeaponFired>();
            // Add a burst
            for (int i = 0; i < bullets; i++)
            {
                var lastWeaponFiredTime = weaponFireds.LastOrDefault()?.Time ?? startTime;
                var timeBetweenShots = rnd.Next(minTimeBetweenShots, maxTimeBetweenShots + 1);
                var time = lastWeaponFiredTime + timeBetweenShots;
                weaponFireds.Add(CreateWeaponFired(playerId, round, weapon, time));
            }

            return weaponFireds;
        }

        private WeaponFired CreateWeaponFired(long playerId, short round, EquipmentElement weapon, int time)
        {
            var weaponFired = new WeaponFired
            {
                PlayerId = playerId,
                Round = round,
                Weapon = weapon,
                Time = time,
            };
            return weaponFired;
        }
    }
}
