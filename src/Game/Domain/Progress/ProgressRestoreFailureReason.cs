namespace Game.Domain.Progress
{
    public enum ProgressRestoreFailureReason
    {
        None = 0,
        ValidationFailed = 1,
        NotApplied = 2,
        ApplicationFailed = 3
    }
}
