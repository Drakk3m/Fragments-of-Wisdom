namespace Game.Domain.Classes
{
    /// <summary>Facts from one mastery mutation, not an Echoes award or eligibility decision.</summary>
    public sealed class MasteryProgressResult
    {
        public bool Applied { get { return AppliedProgress > 0; } }
        // Retain the original input for diagnostics, including rejected nonpositive values.
        public int RequestedProgress { get; }
        public int AppliedProgress { get; }
        public int OverflowProgress { get; }
        public bool ReachedMastery { get; }

        internal MasteryProgressResult(
            int requestedProgress,
            int appliedProgress,
            int overflowProgress,
            bool reachedMastery)
        {
            RequestedProgress = requestedProgress;
            AppliedProgress = appliedProgress;
            OverflowProgress = overflowProgress;
            ReachedMastery = reachedMastery;
        }

        internal static MasteryProgressResult Rejected(int requestedProgress)
        {
            return new MasteryProgressResult(requestedProgress, 0, 0, false);
        }
    }
}
