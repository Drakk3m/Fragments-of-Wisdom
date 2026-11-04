namespace Game.Domain.Progress
{
    /// <summary>Compatibility values corresponding to ProgressRestoreFailureReason.</summary>
    public enum V4ProgressRestoreFailureReason
    {
        None = (int)ProgressRestoreFailureReason.None,
        ValidationFailed = (int)ProgressRestoreFailureReason.ValidationFailed,
        NotApplied = (int)ProgressRestoreFailureReason.NotApplied,
        ApplicationFailed = (int)ProgressRestoreFailureReason.ApplicationFailed
    }
}
