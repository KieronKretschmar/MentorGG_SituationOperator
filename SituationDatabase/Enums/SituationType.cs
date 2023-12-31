﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationDatabase.Enums
{
    /// <summary>
    /// Identifies a type of situation, that is identified by a particular pattern.
    /// </summary>
    public enum SituationType : int
    {
        Unknown = 0,


        // 21XXXX For SinglePlayer - Highlights
        EffectiveHeGrenade = 210001,
        KillWithOwnFlashAssist = 210002,
        Clutch = 210003,
        HighImpactRound = 210004,
        MultiKill = 210005,
        TradeKill = 210006,
        KillThroughSmoke = 210007,
        WallBangKill = 210008,
        CollateralKill = 210009,
        FlashAssist = 210010,

        // 22XXXX For Team - Highlights


        // 51XXXX For SinglePlayer - Misplays
        SmokeFail = 510001,
        DeathInducedBombDrop = 510002,
        SelfFlash = 510003,
        TeamFlash = 510004,
        RifleFiredWhileMoving = 510005,
        UnnecessaryReload = 510006,
        PushBeforeSmokeDetonated = 510007,
        BombDropAtSpawn = 510008,
        HasNotBoughtDefuseKit = 510009,
        MissedTradeKill = 510010,

        // 52XXXX For Team - Misplays

    }
}
