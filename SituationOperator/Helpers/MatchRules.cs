namespace SituationOperator.Helpers
{
    /// <summary>
    /// Holds data about the settings and rules a match was played under.
    /// </summary>
    public struct MatchRules
    {
        public MatchRules(int freezeTime, int roundTimer, int c4Timer)
        {
            FreezeTime = freezeTime;
            RoundTimer = roundTimer;
            C4Timer = c4Timer;
        }

        public int FreezeTime { get; set; }
        public int RoundTimer { get; set; }
        public int C4Timer { get; set; }
    }
}
